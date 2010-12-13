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
    class PageHeaderFooter : ReportElement
    {
        public enum HeaderFooterEnum
        {
            Header,
            Footer
        };

        private ReportItems _reportItems = null;
        private Size _height = null;
        bool _printOnFirstPage = true;
        bool _printOnLastPage = true;
        HeaderFooterEnum _headerFooter;

        public PageHeaderFooter(XmlNode node, ReportElement parent, HeaderFooterEnum headerFooter)
            : base(node, parent)
        {
            _container = true;
            _headerFooter = headerFooter;
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
                case "printonfirstpage":
                    _printOnFirstPage = bool.Parse(attr.InnerText);
                    break;
                case "printonlastpage":
                    _printOnLastPage = bool.Parse(attr.InnerText);
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
            Rdl.Render.FixedContainer headerBox = box.AddFixedContainer(this, Style, context);
            headerBox.Name = "Page" + _headerFooter.ToString();
            headerBox.Height = _height.points;
            headerBox.MatchParentWidth = true;

            if (_reportItems != null)
                _reportItems.Render(headerBox, context);
        }

        public bool PrintOnFirstPage
        {
            get { return _printOnFirstPage; }
        }

        public bool PrintOnLastPage
        {
            get { return _printOnLastPage; }
        }
    }
}
