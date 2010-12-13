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
    class Matrix : DataRegion
    {
        public enum LayoutDirectionEnum
        {
            LTR,
            RTL
        };

        public enum CellDataElementOutputEnum
        {
            Output,
            NoOutput
        };

        // Report elements
        ReportItems _corner = null;
        List<ColumnGrouping> _columnGroupings = new List<ColumnGrouping>();
        List<RowGrouping> _rowGroupings = new List<RowGrouping>();
        Rows _matrixRows = null;
        List<Column> _columns = new List<Column>();
        LayoutDirectionEnum _layoutDirection = LayoutDirectionEnum.LTR;
        int _groupsBeforeRowHeaders = 0;
        string _cellDataElementName = "Cell";
        CellDataElementOutputEnum _cellDataElementOutput = CellDataElementOutputEnum.Output;


        public Matrix(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
            _container = true;
            _cell = true;

            // We render the row groups first, followed by the column groups
            // and finally the details.
            for (int i = 0; i < _columnGroupings.Count - 1; i++)
            {
                _columnGroupings[i].RenderNext = _columnGroupings[i + 1];
                _columnGroupings[i + 1].Parent = _columnGroupings[i];
            }
            for (int i = 0; i < _rowGroupings.Count - 1; i++)
            {
                _rowGroupings[i].RenderNext = _rowGroupings[i + 1];
                _rowGroupings[i + 1].Parent = _rowGroupings[i];
            }

            if (_rowGroupings.Count > 0)
                if (_columnGroupings.Count > 0)
                    // For the purpose of Context, the parent of the column groupings remains the matrix.
                    _rowGroupings[_rowGroupings.Count - 1].RenderNext = _columnGroupings[0];
                else
                {
                    _rowGroupings[_rowGroupings.Count - 1].RenderNext = _matrixRows;
                    _matrixRows.Parent = _rowGroupings[_rowGroupings.Count - 1];
                }
            if (_columnGroupings.Count > 0)
            {
                _columnGroupings[_columnGroupings.Count - 1].RenderNext = _matrixRows;
                _matrixRows.Parent = _columnGroupings[_columnGroupings.Count - 1];
            }
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            base.ParseAttribute(attr);
            switch (attr.Name.ToLower())
            {
                case "corner":
                    _corner = new ReportItems(attr.ChildNodes[0], this);
                    break;
                case "columngroupings":
                    foreach (XmlNode child in attr.ChildNodes)
                        _columnGroupings.Add(ColumnGrouping.GetColumnGrouping(child, this));
                    break;
                case "rowgroupings":
                    foreach (XmlNode child in attr.ChildNodes)
                        _rowGroupings.Add(RowGrouping.GetRowGrouping(child, this));
                    break;
                case "matrixrows":
                    _matrixRows = new Rows(attr, this);
                    break;
                case "matrixcolumns":
                    foreach (XmlNode child in attr.ChildNodes)
                        _columns.Add(new Column(child, this));
                    break;
                case "layoutdirection":
                    _layoutDirection = (LayoutDirectionEnum)Enum.Parse(typeof(LayoutDirectionEnum), attr.InnerText);
                    break;
                case "groupsbeforerowheaders":
                    _groupsBeforeRowHeaders = int.Parse(attr.InnerText);
                    break;
                case "celldataelementname":
                    _cellDataElementName = attr.InnerText;
                    break;
                case "celldataelementoutput":
                    _cellDataElementOutput = (CellDataElementOutputEnum)Enum.Parse(typeof(CellDataElementOutputEnum), attr.InnerText); 
                    break;
                default:
                    break;
            }
        }

        public decimal ColumnWidth
        {
            get
            {
                decimal width = 0;
                foreach (Column c in _columns)
                    width += c.Width;
                return width;
            }
        }

        public int HeaderRows
        {
            // Each column grouping will take up one header row.
            get { return _columnGroupings.Count; }
        }

        public List<Column> Columns
        {
            get { return _columns; }
        }

        public Rows Rows
        {
            get { return _matrixRows; }
        }

        protected override void Render1(Rdl.Render.Container parentBox, Rdl.Runtime.Context context, bool visible)
        {
            base.Render1(parentBox, context, visible);
            if (visible)
            {
                _box = parentBox.AddFlowContainer(this, Style, _context, Rdl.Render.FlowContainer.FlowDirectionEnum.TopDown);
                _box.Name = _name;
            }
        }

        protected override void Render2(Rdl.Runtime.Context context)
        {
            base.Render2(context);
            if (_box != null)
            {
                Rdl.Render.FlowContainer matrixBox = _box as Rdl.Render.FlowContainer;

                // Create the column header row
                Rdl.Render.FlowContainer headerRow = matrixBox.AddFlowContainer(this, Style, _context, Rdl.Render.FlowContainer.FlowDirectionEnum.LeftToRight);
                headerRow.Name = "RowHeaders";
                headerRow.ContextBase = true;

                // Create the corner
                Rdl.Render.Container corner = headerRow.AddFixedContainer(this, Style, _context);
                corner.Name = "Corner";
                if (_rowGroupings.Count > 0)
                    corner.Width = _rowGroupings[0].TotalWidth;
                corner.CanGrowHorizonally = false;
                //corner.CanGrowVertically = false;
                corner.MatchParentHeight = true;
                _corner.Render(corner, _context);

                // Render the column headers
                if ( _columnGroupings.Count > 0)
                    _columnGroupings[0].RenderHeader(headerRow, _context, 0);

                // The groups are linked together into a list, so we need only call
                // render on the first entry in the list.
                if (_rowGroupings.Count > 0)
                    _rowGroupings[0].Render(matrixBox, _context);
                else if (_columnGroupings.Count > 0)
                    _columnGroupings[0].Render(matrixBox, _context, null, 0);
                else _matrixRows.Render(matrixBox, _context);

                matrixBox.Parent.Height = Math.Max(matrixBox.Parent.Height, matrixBox.Top + matrixBox.Height);
                matrixBox.Parent.Width = Math.Max(matrixBox.Parent.Width, matrixBox.Left + matrixBox.Width);
            }
        }
    }
}
