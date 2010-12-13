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

namespace Rdl.Engine.Matrix
{
    class StaticColumns : ColumnGrouping
    {
        List<ReportItems> _staticColumns = new List<ReportItems>();

        public StaticColumns(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
            _container = true;
            _cell = true;
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            base.ParseAttribute(attr);
            switch (attr.Name.ToLower())
            {
                case "staticcolumn":
                    _staticColumns.Add(new ReportItems(attr.ChildNodes[0], this));
                    break;
                default:
                    break;
            }
        }

        internal override int RenderHeader(Rdl.Render.Container box, Rdl.Runtime.Context context, int column)
        {
            for( int i=0; i < _staticColumns.Count; i++)
            {
                ReportItems item = _staticColumns[i];
                Rdl.Render.FixedContainer contentBox = null;

                if (box != null)
                {
                    contentBox = box.AddFixedContainer(this, Style, context);
                    contentBox.Name = "StaticColumnContent";
                    contentBox.Height = Height.points;
                    contentBox.MatchParentHeight = true;
                    contentBox.Width = FindMatrix(this).Columns[i].Width;
                    contentBox.CanGrowVertically = false;
                }

                item.Render(contentBox, context);
            }

            if (RenderNext != null && RenderNext is RowGrouping)
                column = ((ColumnGrouping)RenderNext).RenderHeader(box, context, column);
            else if (RenderNext != null)
                column++;

            return column;
        }

        internal override int Render(Rdl.Render.Container box, Rdl.Runtime.Context context, Rdl.Runtime.Context rowContext, int column)
        {
            if (_staticColumns.Count > FindMatrix(this).Columns.Count)
                throw new Exception("There are more static column elements defined in matrix " + FindMatrix(this).Name + " than there are data columns.");

            if (RenderNext != null)
                if (RenderNext is ColumnGrouping)
                    column = ((ColumnGrouping)RenderNext).Render(box, context, rowContext, column);
                else
                {
                    RenderNext.Render(box, context.Intersect(rowContext));
                    column++;
                }

            return column;
        }
    }
}
