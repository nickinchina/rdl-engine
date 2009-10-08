using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Rdl.Render.Table
{
    public class Rows
    {
        List<Row> _rows = new List<Row>();
        private Container _table = null;

        public Rows(Container table)
        {
            _table = table;
        }

        public IEnumerator GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        public Row this[int index]
        {
            get
            {
                while (index >= _rows.Count)
                    _rows.Add(new Row(_rows.Count, _table));
                return _rows[index];
            }
            set
            {
                while (index >= _rows.Count)
                    _rows.Add(new Row(_rows.Count, _table));
                _rows[index] = value;
            }
        }

        public int Count
        {
            get { return _rows.Count; }
        }
    }

}
