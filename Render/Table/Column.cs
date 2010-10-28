using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Render.Table
{
    public class Column
    {
        decimal _width = 0;
        public List<Cell> Cells = new List<Cell>();
        public int Index = 0;
        private Container _table = null;
        public bool HideIfEmpty = false;
        //public Toggles Toggles = new Toggles();

        public Column(int index, Container table)
        {
            Index = index;
            _table = table;
        }

        public decimal Left
        {
            get
            {
                decimal pos = 0;
                for (int i = 0; i < Index; i++)
                    pos += _table.Columns[i].Width;

                return pos;
            }
        }

        public decimal Width
        {
            set { _width = value; }
            get
            {
//                if (!HideIfEmpty)
                    return _width;
//                foreach (Cell c in Cells)
//                    if (c.Toggles.IsVisible)
//                        return _width;
//                return 0;
            }
        }
    }
}
