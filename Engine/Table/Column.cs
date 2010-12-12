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
