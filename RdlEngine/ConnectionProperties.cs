using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
{
    class ConnectionProperties : ReportElement
    {
        private string _dataProvider = null;
        private Expression _connectString = null;
        private bool _integratedSecurity = false;
        private string _prompt = null;

        public ConnectionProperties(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "dataprovider":
                    _dataProvider = attr.InnerText;
                    break;
                case "connectstring":
                    _connectString = new Expression(attr, this);
                    break;
                case "integratedsecurity":
                    _integratedSecurity = bool.Parse(attr.InnerText);
                    break;
                case "prompt":
                    _prompt = attr.InnerText;
                    break;
                default:
                    break;
            }
        }

        public string DataProvider
        {
            get { return _dataProvider; }
        }

        public string ConnectString
        {
            get { return _connectString.ExecAsString(); }
        }

        public bool IntegratedSecurity
        {
            get { return _integratedSecurity; }
        }

        public string Promt
        {
            get { return _prompt; }
        }
    }
}
