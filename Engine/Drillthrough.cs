using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine
{
    class Drillthrough : ReportElement
    {
        private string _reportName = string.Empty;
        private List<Parameter> _parameters = null;

        public Drillthrough(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "reportname":
                    _reportName = attr.InnerText;
                    break;
                case "parameters":
                    _parameters = new List<Parameter>();
                    foreach (XmlNode child in attr.ChildNodes)
                        _parameters.Add(new Parameter(child, this));
                    break;
                default:
                    break;
            }
        }
    }
}
