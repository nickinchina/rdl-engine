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

namespace Rdl.Engine
{
    class Body : ReportElement
    {
        private ReportItems _reportItems = null;
        private Size _height = null;
        private Int16 _columns = 1;
        private Size _columnSpacing = new Size("0.5in");

        public Body(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
            _container = true;
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "reportitems":
                    _reportItems = new ReportItems(attr, this);
                    break;
                case "height":
                    _height = new Size(attr.InnerText);
                    break;
                case "columns":
                    _columns = Int16.Parse(attr.InnerText);
                    break;
                case "columnspacing":
                    _columnSpacing = new Size(attr.InnerText);
                    break;
                case "style":
                    _style = new Style(attr, this);
                    break;
                default:
                    break;
            }
        }

        internal override void Render(Rdl.Render.Container box, Rdl.Runtime.Context context)
        {
            Rdl.Render.FixedContainer bodyBox = null;

            if (box == null)
                bodyBox = new Rdl.Render.FixedContainer(null, this, new Rdl.Render.BoxStyle(Style, context));
            else
                bodyBox = box.AddFixedContainer(this, Style, context);

            bodyBox.Name = "Body";
            bodyBox.Columns = _columns;
            bodyBox.ColumnSpacing = _columnSpacing.points;
            bodyBox.Height = _height.points;
            bodyBox.Width = box.Width;

            if (_columns > 1)
            {
                bodyBox.Width = (bodyBox.Width / _columns) -
                    (_columnSpacing.points * (_columns - 1));
            }

            _reportItems.Render(bodyBox, context);
        }
    }
}
