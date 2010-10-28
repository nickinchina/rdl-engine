using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine.Chart
{
    class GridLines : ChartElement
    {
        private bool _showGridLines = false;

        public GridLines(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            base.ParseAttribute(attr);
            switch (attr.Name.ToLower())
            {
                case "showgridlines":
                    _showGridLines = bool.Parse(attr.InnerText);
                    break;
                case "style":
                    _style = new Style(attr, this);
                    break;
                default:
                    break;
            }
        }

        public bool ShowGridLines
        {
            get { return _showGridLines; }
        }
    }
}
