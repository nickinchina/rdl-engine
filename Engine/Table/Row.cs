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

namespace Rdl.Engine.Table
{
    class Row : ReportElement
    {
        private List<Cell> _tableCells = new List<Cell>();
        private Size _height = Size.ZeroSize;
        private Visibility _visibility = Visibility.Visible;

        public Row( XmlNode node, ReportElement parent ) :
            base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "tablecells":
                    Int16 i = 0;
                    foreach (XmlNode child in attr.ChildNodes)
                    {
                        Cell c = new Cell(child, this, i);
                        _tableCells.Add(c);
                        i += c.ColSpan;
                    }
                    break;
                case "height":
                    _height = new Size(attr.InnerText);
                    break;
                case "visibility":
                    _visibility = new Visibility(attr, this);
                    break;
                default:
                    break;
            }
        }

        internal override void Render(Rdl.Render.Container box, Rdl.Runtime.Context context)
        {
            Rdl.Render.FixedContainer rowBox = null;
            bool visible = true;
            TextBox tb = FindToggleItem(_visibility);
            if (_visibility != null && _visibility.IsHidden(context) && _visibility.ToggleItem == null)
                visible = false;
            if (box != null && visible)
            {
                rowBox = box.AddFixedContainer(this, Style, context);
                rowBox.Name = "TableRow";
                rowBox.Width = box.Width;
                rowBox.Height = _height.points;

                if (tb != null)
                    tb.LinkedToggles.Add(new Toggle(rowBox, tb));
            }
            decimal cellPos = 0;
            foreach (Cell tc in _tableCells)
                tc.Render(rowBox, context, ref cellPos);
        }
    }
}
