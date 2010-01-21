using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine.Chart
{
    class ChartSeries : ChartElement
    {
        public enum PlotTypeEnum
        {
            Auto,
            Line
        };

        private List<DataPoint> _dataPoints = new List<DataPoint>();
        private PlotTypeEnum _plotType = PlotTypeEnum.Auto;

        public ChartSeries(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            base.ParseAttribute(attr);
            switch (attr.Name.ToLower())
            {
                case "datapoints":
                    foreach (XmlElement child in attr.ChildNodes)
                        _dataPoints.Add(new DataPoint(child, this));
                    break;
                case "plottype":
                    _plotType = (PlotTypeEnum)Enum.Parse(typeof(PlotTypeEnum), attr.InnerText,true);
                    break;
                default:
                    break;
            }
        }

        public List<DataPoint> DataPoints
        {
            get { return _dataPoints; }
        }

        public PlotTypeEnum PlotType
        {
            get { return _plotType; }
        }
    }
}
