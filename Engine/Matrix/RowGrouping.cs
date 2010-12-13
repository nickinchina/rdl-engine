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
    abstract class RowGrouping : MatrixElement
    {
        protected Size _width = Size.ZeroSize;
        protected bool _fixedHeader = false;

        public static RowGrouping GetRowGrouping(XmlNode node, ReportElement parent)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(node.OwnerDocument.NameTable);
            ns.AddNamespace("rdl", parent.Report.XmlNameSpace);

            if (node.SelectSingleNode("rdl:DynamicRows", ns) != null)
                return new DynamicRows(node, parent);
            else
                return new StaticRows(node, parent);
        }

        public RowGrouping(XmlNode node, ReportElement parent)
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
                case "dynamicrows":
                case "staticrows":
                    foreach (XmlNode child in attr.ChildNodes)
                        ParseAttribute(child);
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

        public decimal TotalWidth
        {
            get
            {
                MatrixElement next = RenderNext;
                decimal width = Width.points;
                while (next != null && next is RowGrouping)
                {
                    width += ((RowGrouping)next).Width.points;
                    next = ((RowGrouping)next).RenderNext;
                }
                return width;
            }
        }
    }
}
