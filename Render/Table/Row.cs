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
