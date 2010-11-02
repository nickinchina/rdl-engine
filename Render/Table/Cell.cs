using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Render.Table
{
    public class Cell : Rdl.Render.Container
    {
        public int Row = 0;
        public int Column = 0;
        public int RowSpan = 1;
        public int ColSpan = 1;

        public Cell(Container parent, Rdl.Engine.ReportElement reportElement, BoxStyle style)
            : base(parent, reportElement, style)
        {
        }

        public Cell(Cell b)
            : this(b, false)
        {
        }

        public Cell(Cell b, bool copyChildren)
            : base(b, copyChildren)
        {
            Row = b.Row;
            Column = b.Column;
            RowSpan = b.RowSpan;
            ColSpan = b.ColSpan;
        }

        public Container Table
        {
            get { return Parent as Container; }
        }

        public override decimal Width
        {
            get
            {
                Container table = Parent as Container;
                if (table != null)
                {
                    decimal width = 0;
                    for (int i = 0; i < ColSpan; i++)
                        width += table.Columns[Column + i].Width;
                    return width;
                }
                return base.Width;
            }
            set
            {
                Container table = Parent as Container;
                if (table != null && ColSpan == 1)
                    table.Columns[Column].Width = Math.Max(table.Columns[Column].Width, value);
                base.Width = value;
            }
        }

        public override decimal Height
        {
            get
            {
                Container table = Parent as Container;
                if (table != null)
                {
                    decimal height = 0;
                    for (int i = 0; i < RowSpan; i++)
                        height += table.Rows[Row + i].Height;
                    return height;
                }
                return base.Height;
            }
            set
            {
                Container table = Parent as Container;
                if (table != null && RowSpan == 1)
                    table.Rows[Row].Height = Math.Max(table.Rows[Row].Height, value);
                base.Height = value;
            }
        }

        public override decimal Top
        {
            get
            {
                if (Parent is Container)
                    base.Top = ((Container)Parent).Rows[Row].Top;
                return _top;
            }
            set
            {
                base.Top = value;
            }
        }

        public override decimal Left
        {
            get
            {
                if (Parent is Container)
                    base.Left = ((Container)Parent).Columns[Column].Left;
                return _left;
            }
            set
            {
                base.Left = value;
            }
        }
    }
}
