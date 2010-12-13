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
using System.Text;

namespace Rdl.Render.Table
{
    public class Row
    {
        public decimal _height = 0;
        public List<Cell> Cells = new List<Cell>();
        public int Index = 0;
        private Container _table = null;
        //public Toggles Toggles = new Toggles();
        public bool HideIfEmpty = false;

        public Row(int index, Container table)
        {
            Index = index;
            _table = table;
        }

        public decimal Top
        {
            get
            {
                decimal pos = 0;
                for (int i = 0; i < Index; i++)
                    pos += _table.Rows[i].Height;

                return pos;
            }
        }

        public decimal Height
        {
            set { _height = value; }
            get
            {
//                if (!HideIfEmpty)
                    return _height;
                //foreach (Cell c in Cells)
                //    if (c.Toggles.IsVisible)
                //        return _height;
                //return 0;
            }
        }
    }

}
