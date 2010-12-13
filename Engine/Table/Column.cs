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
    class Column : ReportElement
    {
        private Size _width = Size.ZeroSize;
        private Visibility _visibility = Visibility.Visible;
        private bool _fixedHeader = false;

        public Column(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "width":
                    _width = new Size(attr.InnerText);
                    break;
                case "visibility":
                    _visibility = new Visibility(attr, this);
                    break;
                case "fixedheader":
                    _fixedHeader = bool.Parse(attr.InnerText);
                    break;
                default:
                    break;
            }
        }

        public Size Width
        {
            get { return _width; }
        }

        public Visibility Visibility
        {
            get { return _visibility; }
        }

        public bool FixedHeader
        {
            get { return _fixedHeader; }
        }
    }
}
