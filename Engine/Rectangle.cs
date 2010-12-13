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
    class Rectangle : ReportItem
    {
        bool _pageBreakAtStart = false;
        bool _pageBreakAtEnd = false;
        ReportItems _reportItems = null;

        public Rectangle(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
            _container = true;
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            base.ParseAttribute(attr);
            switch (attr.Name.ToLower())
            {
                case "reportitems":
                    _reportItems = new ReportItems(attr, this);
                    break;
                case "pagebreakatstart":
                    _pageBreakAtStart = bool.Parse(attr.InnerText);
                    break;
                case "pagebreakatend":
                    _pageBreakAtEnd = bool.Parse(attr.InnerText);
                    break;
                default:
                    break;
            }
        }

        protected override void Render1(Rdl.Render.Container box, Rdl.Runtime.Context context, bool visible)
        {
            if (visible)
            {
                _box = box.AddFixedContainer(this, Style, context);
                _box.PageBreakBefore = _pageBreakAtStart;
                _box.PageBreakAfter = _pageBreakAtEnd;
                _box.KeepTogether = true;
                _box.Name = "Rectangle";
            }
        }

        protected override void Render2(Rdl.Runtime.Context context)
        {
            if (_reportItems != null)
                _reportItems.Render((Rdl.Render.Container)_box, context);
        }
    }
}
