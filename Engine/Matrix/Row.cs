using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Rdl.Engine;

namespace Rdl.Engine.Matrix
{
    class Row : MatrixElement
    {
        Size _height = Size.ZeroSize;
        List<Cell> _matrixCells = new List<Cell>();

        public Row(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "height":
                    _height = new Size(attr.InnerText);
                    break;
                case "matrixcells":
                    int i = 0;
                    foreach (XmlNode child in attr.ChildNodes)
                        _matrixCells.Add(new Cell(child, this, i++));
                    break;
                default:
                    break;
            }
        }

        internal override void Render(Rdl.Render.Container box, Rdl.Runtime.Context context)
        {
            Rdl.Render.FlowContainer rowBox = null;
            if (box != null)
            {
                rowBox = box.AddFlowContainer(this, Style, context, Rdl.Render.FlowContainer.FlowDirectionEnum.LeftToRight);
                rowBox.Name = "RowBox";
                rowBox.MatchParentWidth = true;
                rowBox.Height = Height.points;
            }

            int index = 0;
            foreach (Cell tc in _matrixCells)
            {
                Rdl.Render.FixedContainer cell = null;

                Matrix matrix = FindMatrix(this);

                if (rowBox != null)
                {
                    cell = rowBox.AddFixedContainer(this, Style, context);
                    cell.Name = "MatrixCell";
                    cell.Height = _height.points;
                    cell.Width = matrix.Columns[index].Width;
                    cell.MatchParentHeight = true;
                    cell.CanGrowHorizonally = false;
                }

                tc.Render(cell, context);

                index++;
            }
        }

        public Size Height
        {
            get { return _height; }
        }
    }
}
