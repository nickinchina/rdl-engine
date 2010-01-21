using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Rdl.Render
{
    class TableContainer : Container
    {


        public TableRows Rows = null;
        public new TableColumns Columns = null;

        public TableContainer(Container parent, Rdl.Engine.ReportElement reportElement, BoxStyle style)
            : base(parent, reportElement, style)
        {
            Rows = new TableRows(this);
            Columns = new TableColumns(this);
        }

        public Cell AddCell(int row, int column, Rdl.Engine.ReportElement reportElement, Rdl.Engine.Style style)
        {
            Cell cell = new Cell(this, reportElement, new BoxStyle(style));

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

        public override decimal Height
        {
            get
            {
                decimal height = 0;
                foreach (Row r in Rows)
                    if (r.Toggle == null || r.Toggle.ToggleState == TextElement.ToggleStateEnum.open)
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
                    if (c.Toggle == null || c.Toggle.ToggleState == TextElement.ToggleStateEnum.open)
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
