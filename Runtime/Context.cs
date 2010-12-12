using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Runtime
{
    class GroupEntry
    {
        public List<System.Data.DataRow> Rows = new List<System.Data.DataRow>();
        public Context ChildContext = null;
        public Rdl.Engine.Grouping Grouping = null;
        public List<Rdl.Engine.SortBy> Sorting = null;
    }

    internal class ContextState
    {
        public int CurrentGroup;
        public int CurrentPosition;

        public ContextState(int currentPosition, int currentGroup)
        {
            CurrentGroup = currentGroup;
            CurrentPosition = currentPosition;
        }
    }

    public class Context
    {
        private Rdl.Engine.DataSet _dataSet = null;
        private Rdl.Engine.Filters _filters = null;
        private Rdl.Engine.Grouping _grouping = null;
        private List<Rdl.Engine.SortBy> _sorting = null;
        private Rdl.Engine.Filter _groupFilter = null;
        private List<GroupEntry> _groups = new List<GroupEntry>();
        private List<System.Data.DataRow> _rows = null;
        private System.Data.DataRow _currentRow = null;
        private Context _parentContext;
        private Int32 _currentPosition;
        private Int32 _currentGroup;
        private List<Rdl.Engine.TextBox> _toggleSource = new List<Rdl.Engine.TextBox>();
        private string _reportItemName = null;
        internal Dictionary<string, Rdl.Engine.TextBox> TextBoxes = new Dictionary<string, Rdl.Engine.TextBox>();

        private Context()
        {
        }

        internal Context(Context parentContext,
            Rdl.Engine.DataSet dataSet,
            Rdl.Engine.Filters filters,
            Rdl.Engine.Grouping grouping,
            List<Rdl.Engine.SortBy> sorting
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
                if (parentContext == null)
                    foreach (System.Data.DataRow row in dataSet.Table.Rows)
                        TestAddRow(row);
                else
                    foreach (System.Data.DataRow row in parentContext.Rows)
                        TestAddRow(row);
            }
            else if (parentContext != null)
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
                _groups.Add(new GroupEntry());
                _currentGroup = 0;
                System.Data.DataRow lastRow = null;
                foreach (System.Data.DataRow row in _rows)
                {
                    if (lastRow != null)
                        if (CompareForGroup(lastRow, row) != 0)
                        {
                            _currentGroup++;
                            _groups.Add(new GroupEntry());
                        }
                    _groups[_currentGroup].Rows.Add(row);
                    lastRow = row;
                }

                if (_grouping.Filters != null)
                    _groupFilter = _grouping.Filters.GroupFilter;
            }
            else if (_rows.Count > 0)
            {
                GroupEntry ge = new GroupEntry();
                ge.Rows = _rows;
                _groups.Add(ge);
            }

            // Remove rows not matching the group filter operator.
            if (_groupFilter != null)
            {
                foreach (GroupEntry ge in _groups)
                {
                    List<System.Data.DataRow> rows = ge.Rows;

                    // Sort the list or rows based on the count expression.
                    rows.Sort(CompareForGroupFilter);
                    Int32 ct = 0;
                    float v = 0;
                    if (_groupFilter.Operator == Rdl.Engine.Filter.FilterOperator.TopN ||
                        _groupFilter.Operator == Rdl.Engine.Filter.FilterOperator.BottomN)
                        Int32.TryParse(_groupFilter.GroupValue.ToString(), out ct);

                    if (_groupFilter.Operator == Rdl.Engine.Filter.FilterOperator.TopPercent ||
                        _groupFilter.Operator == Rdl.Engine.Filter.FilterOperator.BottomPercent)
                    {
                        float.TryParse(_groupFilter.GroupValue.ToString(), out v);
                        ct = (Int32)((float)rows.Count * v / 100F);
                    }
                    if (ct < rows.Count)
                        rows.RemoveRange(0, ct);
                }
            }

            // Sort the resulting lists according to the sort expressions.
            if (_sorting != null && _sorting.Count > 0)
            {
                _groups.Sort(CompareForGroupSort);
                for(_currentGroup=0; _currentGroup < _groups.Count; _currentGroup++)
                    _groups[_currentGroup].Rows.Sort(CompareForSort);
            }

            _currentGroup = 0;
            if (_groups.Count > 0)
                _rows = _groups[0].Rows;
            else
                _rows = new List<System.Data.DataRow>();
            _currentPosition = 0;
            if (_rows.Count > 0)
                _currentRow = _rows[0];
            else
                _currentRow = null;
        }

        public Context Intersect(Context ctxt)
        {
            Context newCtxt = new Context();

            newCtxt._parentContext = this;
            newCtxt._currentPosition = 0;
            newCtxt._dataSet = _dataSet;

            newCtxt._rows = new List<System.Data.DataRow>();

            foreach (System.Data.DataRow row in _rows)
                if (ctxt._rows.Contains(row))
                    newCtxt._rows.Add(row);

            GroupEntry ge = new GroupEntry();
            ge.Rows = newCtxt._rows;
            newCtxt._groups.Add(ge);

            newCtxt._currentGroup = 0;
            newCtxt._rows = newCtxt._groups[0].Rows;
            newCtxt._currentPosition = 0;
            if (newCtxt._rows.Count > 0)
                newCtxt._currentRow = newCtxt._rows[0];
            else
                newCtxt._currentRow = null;

            return newCtxt;
        }

        private void TestAddRow(System.Data.DataRow row)
        {
            _currentRow = row;

            if (_filters != null)
                foreach (Rdl.Engine.Filter filter in _filters)
                {
                    if (!filter.Evaluate(this))
                        return;
                }
            if (_grouping != null && _grouping.Filters != null)
                foreach( Rdl.Engine.Filter filter in _grouping.Filters)
                {
                    if (!filter.Evaluate(this))
                        return;
                }

            _rows.Add(row);
        }

        internal Rdl.Engine.DataSet DataSet
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
                _rows = _groups[_currentGroup].Rows;
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

        public string ReportItemName
        {
            get { return _reportItemName; }
            set { _reportItemName = value; }
        }

        internal Context GetChildContext(
            Rdl.Engine.DataSet dataSet,
            Rdl.Engine.Filters filters,
            Rdl.Engine.Grouping grouping,
            List<Rdl.Engine.SortBy> sorting
            )
        {
            //return new Context(this, dataSet, filters, grouping, sorting);
            if (_groups[_currentGroup].ChildContext == null || _groups[_currentGroup].Grouping != grouping || _groups[_currentGroup].Sorting != sorting)
            {
                Context ctxt = new Context(this, dataSet, filters, grouping, sorting);
                _groups[_currentGroup].ChildContext = ctxt;
                _groups[_currentGroup].Grouping = grouping;
                _groups[_currentGroup].Sorting = sorting;
            }
            _groups[_currentGroup].ChildContext.Reset();
            return _groups[_currentGroup].ChildContext;
        }


        public void Reset()
        {
            _currentGroup = 0;
            _currentPosition = 0;
            _currentRow = null;
            _rows = null;
            if (_groups.Count > 0)
            {
                _rows = _groups[0].Rows;
                if (_rows.Count > 0)
                    _currentRow = _rows[0];
            }
        }

        private int CompareForGroup(System.Data.DataRow row1, System.Data.DataRow row2)
        {
            int r = 0;
            foreach( Rdl.Engine.Expression exp in _grouping.GroupExpressions)
            {
                _currentRow = row1;
                object obj1 = exp.Exec(this);
                _currentRow = row2;
                object obj2 = exp.Exec(this);

                r = Compare.CompareTo(obj1, obj2);
                if (r != 0)
                    return r;
            }
            return r;
        }

        public string CurrentGroupValue
        {
            get
            {
                string value = string.Empty;
                if (_grouping != null)
                    foreach (Rdl.Engine.Expression exp in _grouping.GroupExpressions)
                    {
                        if (value != string.Empty)
                            value += " - ";
                        value += exp.Exec(this).ToString();
                    }
                return value;
            }
        }


        private int CompareForGroupFilter(System.Data.DataRow row1, System.Data.DataRow row2)
        {
            _currentRow = row1;
            object obj1 = _groupFilter.FilterExpression.Exec(this);
            _currentRow = row2;
            object obj2 = _groupFilter.FilterExpression.Exec(this);

            if (_groupFilter.Operator == Rdl.Engine.Filter.FilterOperator.TopPercent ||
                _groupFilter.Operator == Rdl.Engine.Filter.FilterOperator.TopN)
                return Compare.CompareTo(obj2, obj1);
            else
                return Compare.CompareTo(obj1, obj2);
        }

        private int CompareForSort(System.Data.DataRow row1, System.Data.DataRow row2)
        {
            foreach (Rdl.Engine.SortBy sortBy in _sorting)
            {
                _currentRow = row1;
                object obj1 = sortBy.Expression.Exec(this);
                _currentRow = row2;
                object obj2 = sortBy.Expression.Exec(this);

                int ret = Compare.CompareTo(obj1, obj2);
                if (ret != 0)
                {
                    if (sortBy.Direction == Rdl.Engine.SortBy.SortDirection.Descending)
                        ret = 0 - ret;
                    return ret;
                }
            }
            return 0;
        }

        private int CompareForGroupSort(GroupEntry ge1, GroupEntry ge2)
        {
            foreach (Rdl.Engine.SortBy sortBy in _sorting)
            {
                _rows = ge1.Rows;
                _currentRow = _rows[0];
                object obj1 = sortBy.Expression.Exec(this);
                _rows = ge2.Rows;
                _currentRow = _rows[0];
                object obj2 = sortBy.Expression.Exec(this);

                int ret = Compare.CompareTo(obj1, obj2);
                if (ret != 0)
                {
                    if (sortBy.Direction == Rdl.Engine.SortBy.SortDirection.Descending)
                        ret = 0 - ret;
                    return ret;
                }
            }
            return 0;
        }

        public Context FindContextByGroupName(string name, Rdl.Engine.Report rpt)
        {
            if (rpt.DataSets[name] != null)
                return new Context(null, rpt.DataSets[name], null, null, null);
            if (_grouping != null)
                if (_grouping.Name == name)
                    return this;
            if (_reportItemName == name)
                return this;
            if (_parentContext != null)
                return (_parentContext.FindContextByGroupName(name, rpt));
            else if (_dataSet.Name == name)
                return this;
            throw new Exception("Invalid context name " + name);
        }

        internal Context FindContextByDS(Rdl.Engine.DataSet ds)
        {
            if (ds == null || ds == _dataSet)
                return this;
            if (_parentContext != null)
                return _parentContext.FindContextByDS(ds);
            return null;
        }

        public List<System.Data.DataRow> Rows
        {
            get { return _rows; }
        }

        public void LinkToggles()
        {
            foreach (Rdl.Engine.TextBox tb in TextBoxes.Values)
            {
                foreach (Rdl.Engine.Toggle tog in tb.LinkedToggles)
                {
                    Rdl.Render.Container c = (Rdl.Render.Container)tog.Element;

                    c.Toggles.AddToggle(tb.TextElement, tog.Direction);
                    tb.TextElement.LinkedToggles.Add(c);
                    tb.TextElement.IsToggle = true;
                }
                tb.LinkedToggles.Clear();
            }
        }

        internal ContextState ContextState
        {
            get { return new ContextState(_currentPosition, _currentGroup); }
            set
            {
                _currentGroup = value.CurrentGroup;
                if (_groups.Count > 0)
                    _rows = _groups[_currentGroup].Rows;
                else
                    _rows = new List<System.Data.DataRow>();
                _currentPosition = value.CurrentPosition;
                _currentRow = null;
                if (_rows.Count > 0)
                    _currentRow = _rows[0];
            }
        }
    }
}
