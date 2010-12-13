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
