using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine
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
                case "labelfield":
                    _labelField = attr.InnerText;
                    break;
                default:
                    break;
            }
        }

        public string DataSetName
        {
            get { return _dataSetName; }
        }

        public string ValueField
        {
            get { return _valueField; }
        }

        public string LableField
        {
            get { return _labelField; }
        }
    }
}