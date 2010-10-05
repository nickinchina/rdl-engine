using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Render.Table
{
    public class Container : Rdl.Render.Container
    {
        public Rows Rows = null;
        public new Columns Columns = null;

        public Container(Rdl.Render.Container parent, Rdl.Engine.ReportElement reportElement, BoxStyle style)
            : base(parent, reportElement, style)
        {
            Rows = new Rows(this);
            Columns = new Columns(this);
        }

        public Cell AddCell(int row, int column, Rdl.Engine.ReportElement reportElement, Rdl.Engine.Style style, Rdl.Runtime.Context context)
        {
            Cell cell = new Cell(this, reportElement, new BoxStyle(style, context));

            Rows[row].Cells.Add(cell);
            _childElements.Add(cell);
            Columns[column].Cells.Add(cell);
            cell.Row = row;
            cell.Column = column;
            cell.Width = Columns[column].Width;
            cell.Height = Rows[row].Height;
            cell.Name = "Cell";
            cell.CanGrowHorizonally = false;

            return cell;
        }

        public Cell FindCell(int row, int col)
        {
            foreach (Cell c in Rows[row].Cells)
                if (c.Column == col)
                    return c;
            return null;
        }

        public override decimal Height
        {
            get
            {
                decimal height = 0;
                foreach (Row r in Rows)
                    height += r.Height;
                return height;
            }
            set
            {
                base.Height = value;
            }
        }

        public override decimal Width
        {
            get
            {
                decimal width = 0;
                foreach (Column c in Columns)
                    width += c.Width;
                return width;
            }
            set
            {
                base.Width = value;
            }
        }
    }
}
