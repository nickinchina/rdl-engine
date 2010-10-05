using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Rdl.Engine;

namespace Rdl.Engine.Matrix
{
    class Subtotal : MatrixElement
    {
        public enum PositionEnum
        {
            Before,
            After
        };
        public enum DataElementOutputEnum
        {
            Output,
            NoOutput
        };

        ReportItems _reportItems = null;
        PositionEnum _position = PositionEnum.After;
        string _dataElementName = "Total";
        DataElementOutputEnum _dataElementOutput = DataElementOutputEnum.NoOutput;


        public Subtotal(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "reportitems":
                    _reportItems = new ReportItems(attr, Parent);
                    break;
                case "style":
                    _style = new Style(attr, this);
                    break;
                case "position":
                    _position = (PositionEnum)Enum.Parse(typeof(PositionEnum), attr.InnerText);
                    break;
                case "dataelementname":
                    _dataElementName = attr.InnerText;
                    break;
                case "dataelementoutput":
                    _dataElementOutput = (DataElementOutputEnum)Enum.Parse(typeof(DataElementOutputEnum), attr.InnerText);
                    break;
                default:
                    break;
            }
        }

        public ReportItems ReportItems
        {
            get { return _reportItems; }
        }

        public PositionEnum Position
        {
            get { return _position; }
        }

        public DataElementOutputEnum DataElementOutput
        {
            get { return _dataElementOutput; }
        }

    }
}
