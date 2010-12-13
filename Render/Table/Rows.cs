/*-----------------------------------------------------------------------------------
This file is part of the SawikiSoft RDL Engine.
The SawikiSoft RDL Engine is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

The SawikiSoft RDL Engine is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
-----------------------------------------------------------------------------------*/
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
