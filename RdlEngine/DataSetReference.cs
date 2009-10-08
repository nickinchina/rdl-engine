using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
{
    class DataSetReference : ReportElement
    {
        private string _dataSetName = null;
        private string _valueField = null;
        private string _labelField = null;

        public DataSetReference(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "datasetname":
                    _dataSetName = attr.InnerText;
                    break;
                case "valuefield":
                    _valueField = attr.InnerText;
                    break;
                case "labelField":
                    _labelField = attr.InnerText;
                    break;
                default:
                    break;
            }
        }
    }
}