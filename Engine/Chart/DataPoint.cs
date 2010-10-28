using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine.Chart
{
    class DataPoint : ChartElement
    {
        private List<Expression> _dataValues = new List<Expression>();
        private DataLabel _dataLabel = null;
        private Action _action = null;
        private Marker _marker = null;
        private string _dataElementName = string.Empty;
        private Enums.DataElementOutputEnum _dataElementOutput = Enums.DataElementOutputEnum.Output;

        public DataPoint(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            base.ParseAttribute(attr);
            switch (attr.Name.ToLower())
            {
                case "datavalues":
                    foreach (XmlNode child in attr.ChildNodes)
                        _dataValues.Add(new Expression(child, this));
                    break;
                case "datalabel":
                    _dataLabel = new DataLabel(attr, this);
                    break;
                case "action":
                    _action = new Action(attr, this);
                    break;
                case "style":
                    _style = new Style(attr, this);
                    break;
                case "marker":
                    _marker = new Marker(attr, this);
                    break;
                case "dataelementname":
                    _dataElementName = attr.InnerText;
                    break;
                case "dataelementoutput":
                    _dataElementOutput = (Enums.DataElementOutputEnum)Enum.Parse(typeof(Enums.DataElementOutputEnum), attr.InnerText, true);
                    break;
                default:
                    break;
            }
        }

        public List<Expression> DataValues
        {
            get { return _dataValues; }
        }

        public DataLabel DataLabel
        {
            get { return _dataLabel; }
        }

        public Action Action
        {
            get { return _action; }
        }

        public Marker Marker
        {
            get { return _marker; }
        }

        public string DataElementName
        {
            get { return _dataElementName; }
        }

        public Enums.DataElementOutputEnum DataElementOutput
        {
            get { return _dataElementOutput; }
        }
    }   
}
