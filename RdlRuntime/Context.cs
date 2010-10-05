using System;
using System.Collections.Generic;
using System.Text;

namespace RdlRuntime
{
    class Context
    {
        private RdlEngine.DataSet _dataSet = null;
        private RdlEngine.Filters _filters = null;
        private RdlEngine.Grouping _grouping = null;
        private List<RdlEngine.SortBy> _sorting = null;
        private RdlEngine.Filter _groupFilter = null;
        private List<List<System.Data.DataRow>> _groups = new List<List<System.Data.DataRow>>();
        private List<System.Data.DataRow> _rows = null;
        private System.Data.DataRow _currentRow = null;
        private Context _parentContext;
        private Int32 _currentPosition;
        private Int32 _currentGroup;

        public Context(Context parentContext,
            RdlEngine.DataSet dataSet,
            RdlEngine.Filters filters,
            RdlEngine.Grouping grouping,
            List<RdlEngine.SortBy> sorting
            )
        {
            _parentContext = parentContext;
            _filters = filters;
            _grouping = grouping;
            _sorting = sorting;
            _currentPosition = 0;

            _rows = new List<System.Data.DataRow>();

            // Load the datarows into the context
            if (dataSet != null)
            {
                _dataSet = dataSet;
                foreach (System.Data.DataRow row in dataSet.Table.Rows)
                    TestAddRow(row);
            }
            else
            {
                _dataSet = parentContext._dataSet;
                foreach (System.Data.DataRow row in parentContext._rows)
                    TestAddRow(row);
            }

            if (_grouping != null && _rows.Count > 0)
            {
                // Sort the row list by the group expression
                _rows.Sort(CompareForGroup);

                // Build a list of group members for each group instance
                _groups.Add(new List<System.Data.DataRow>());
                _currentGroup = 0;
                System.Data.DataRow lastRow = null;
                foreach (System.Data.DataRow row in _rows)
                {
                    if (lastRow != null)
                        if (CompareForGroup(lastRow, row) != 0)
                        {
                            _currentGroup++;
                            _groups.Add(new List<System.Data.DataRow>());
                        }
                    _groups[_currentGroup].Add(row);
                    lastRow = row;
                }

                if (_grouping.Filters != null)
                    _groupFilter = _grouping.Filters.GroupFilter;
            }
            else
                _groups.Add(_rows);

            // Remove rows not matching the group filter operator.
            if (_groupFilter != null)
            {
                foreach (List<System.Data.DataRow> rows in _groups)
                {
                    // Sort the list or rows based on the count expression.
                    rows.Sort(CompareForGroupFilter);
                    Int32 ct = 0;
                    float v = 0;
                    if (_groupFilter.Operator == RdlEngine.Filter.FilterOperator.TopN ||
                        _groupFilter.Operator == RdlEngine.Filter.FilterOperator.BottomN)
                        Int32.TryParse(_groupFilter.GroupValue.ToString(), out ct);

                    if (_groupFilter.Operator == RdlEngine.Filter.FilterOperator.TopPercent ||
                        _groupFilter.Operator == RdlEngine.Filter.FilterOperator.BottomPercent)
                    {
                        float.TryParse(_groupFilter.GroupValue.ToString(), out v);
                        ct = (Int32)((float)rows.Count * v / 100F);
                    }
                    if (ct < rows.Count)
                        rows.RemoveRange(0, ct);
                }
            }

            // Sort the resulting lists according to the sort expressions.
            if (_sorting != null)
            {
                _groups.Sort(CompareForGroupSort);
                for(_currentGroup=0; _currentGroup < _groups.Count; _currentGroup++)
                    _groups[_currentGroup].Sort(CompareForSort);
            }

            _currentGroup = 0;
            _rows = _groups[0];
            _currentPosition = 0;
            if (_rows.Count > 0)
                _currentRow = _rows[0];
            else
                _currentRow = null;
        }

        private void TestAddRow(System.Data.DataRow row)
        {
            _currentRow = row;

            if (_filters != null)
                foreach (RdlEngine.Filter filter in _filters)
                {
                    if (!filter.Evaluate())
                        return;
                }
            if (_grouping != null && _grouping.Filters != null)
                foreach( RdlEngine.Filter filter in _grouping.Filters)
                {
                    if (!filter.Evaluate())
                        return;
                }

            _rows.Add(row);
        }

        public RdlEngine.DataSet DataSet
        {
            get { return _dataSet; }
        }

        public System.Data.DataRow CurrentRow
        {
            get { return _currentRow; }
        }

        public Int32 RowIndex
        {
            get { return _currentPosition; }
            set 
            { 
                _currentPosition = value;
                if (_currentPosition < _rows.Count)
                    _currentRow = _rows[_currentPosition];
                else
                    _currentRow = null;
            }
        }

        public void MoveNext()
        {
            _currentPosition++;
            if (_currentPosition < _rows.Count)
                _currentRow = _rows[_currentPosition];
            else
                _currentRow = null;
        }

        public void NextGroup()
        {
            _currentGroup++;
            if (_currentGroup < _groups.Count)
            {
                _rows = _groups[_currentGroup];
                _currentPosition = 0;
                _currentRow = _rows[0];
            }
            else
                _currentRow = null;
        }

        public int GroupCount
        {
            get { return _groups.Count; }
        }

        public int GroupIndex
        {
            get { return _currentGroup; }
        }

        public void Reset()
        {
            _currentGroup = 0;
            _currentPosition = 0;
            _rows = _groups[0];
            _currentRow = _rows[0];
        }

        private int CompareForGroup(System.Data.DataRow row1, System.Data.DataRow row2)
        {
            foreach( RdlEngine.Expression exp in _grouping.GroupExpressions)
            {
                _currentRow = row1;
                object obj1 = exp.Exec(this);
                _currentRow = row2;
                object obj2 = exp.Exec(this);

                return RdlEngine.Utility.ApplyCompare(obj1, obj2);
            }
            return 0;
        }


        private int CompareForGroupFilter(System.Data.DataRow row1, System.Data.DataRow row2)
        {
            _currentRow = row1;
            object obj1 = _groupFilter.FilterExpression.Exec(this);
            _currentRow = row2;
            object obj2 = _groupFilter.FilterExpression.Exec(this);

            if (_groupFilter.Operator == RdlEngine.Filter.FilterOperator.TopPercent ||
                _groupFilter.Operator == RdlEngine.Filter.FilterOperator.TopN )
                return RdlEngine.Utility.ApplyCompare(obj2, obj1);
            else
                return RdlEngine.Utility.ApplyCompare(obj1, obj2);
        }

        private int CompareForSort(System.Data.DataRow row1, System.Data.DataRow row2)
        {
            foreach (RdlEngine.SortBy sortBy in _sorting)
            {
                _currentRow = row1;
                object obj1 = sortBy.Expression.Exec(this);
                _currentRow = row2;
                object obj2 = sortBy.Expression.Exec(this);

                int ret = RdlEngine.Utility.ApplyCompare(obj1, obj2);
                if (ret != 0)
                {
                    if (sortBy.Direction == RdlEngine.SortBy.SortDirection.Descending)
                        ret = 0 - ret;
                    return ret;
                }
            }
            return 0;
        }

        private int CompareForGroupSort(List<System.Data.DataRow> rows1, List<System.Data.DataRow> rows2)
        {
            foreach (RdlEngine.SortBy sortBy in _sorting)
            {
                _rows = rows1;
                _currentRow = _rows[0];
                object obj1 = sortBy.Expression.Exec(this);
                _rows = rows2;
                _currentRow = rows2[0];
                object obj2 = sortBy.Expression.Exec(this);

                int ret = RdlEngine.Utility.ApplyCompare(obj1, obj2);
                if (ret != 0)
                {
                    if (sortBy.Direction == RdlEngine.SortBy.SortDirection.Descending)
                        ret = 0 - ret;
                    return ret;
                }
            }
            return 0;
        }

        public Context FindContextByGroupName(string name)
        {
            if (_grouping != null)
                if (_grouping.Name == name)
                    return this;
            if (_parentContext != null)
                return (_parentContext.FindContextByGroupName(name));
            return null;
        }

        public List<System.Data.DataRow> Rows
        {
            get { return _rows; }
        }
    }
}
