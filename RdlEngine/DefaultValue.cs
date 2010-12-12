using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
{
    class DefaultValue : ReportElement
    {
        private DataSetReference _dataSetReference = null;
        private List<Expression> _values = null;

        public DefaultValue(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "datasetreference":
                    _dataSetReference = new DataSetReference(attr, this);
                    break;
                case "values":
                    _values = new List<Expression>();
                    foreach (XmlNode child in attr.ChildNodes)
                        _values.Add(new Expression(child, this));
                    break;
                default:
                    break;
            }
        }
    }
}
