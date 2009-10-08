using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine.Chart
{
    class StaticSeries : SeriesGrouping
    {
        private List<Expression> _labels = new List<Expression>();

        public StaticSeries(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            base.ParseAttribute(attr);
            switch (attr.Name.ToLower())
            {
                case "staticmember":
                    foreach (XmlNode child in attr.ChildNodes)
                        _labels.Add(new Expression(child, this));
                    break;
                default:
                    break;
            }
        }

        public override SeriesList GetSeriesList(Rdl.Runtime.Context context, Series parentSeries)
        {
            List<Series> result = new List<Series>();

            int i = 0;
            foreach (Expression exp in _labels)
            {
                string value = exp.ExecAsString(context);
                result.Add(new Series(context, this, parentSeries, value, i++));
            }

            return new SeriesList(result);
        }
    }
}
