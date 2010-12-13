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
