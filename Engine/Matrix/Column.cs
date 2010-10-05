using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Rdl.Engine;

namespace Rdl.Engine.Matrix
{
    class Column : ReportElement
    {
        Size _width = Size.ZeroSize;

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
                default:
                    break;
            }
        }

        public decimal Width
        {
            get { return _width.points; }
        }
    }
}
