using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Render.Table
{
    public class Columns
    {
        List<Column> _columns = new List<Column>();
        private Container _table = null;

        public Columns(Container table)
        {
            _table = table;
        }

        public IEnumerator GetEnumerator()
        {
            return _columns.GetEnumerator();
        }

        public Column this[int index]
        {
            get
            {
                while (index >= _columns.Count)
                    _columns.Add(new Column(_columns.Count, _table));
                return _columns[index];
            }
            set
            {
                while (index >= _columns.Count)
                    _columns.Add(new Column(_columns.Count, _table));
                _columns[index] = value;
            }
        }

        public int Count
        {
            get { return _columns.Count; }
        }
    }
}
