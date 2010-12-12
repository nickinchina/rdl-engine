using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine.Chart
{
    class DynamicSeries : SeriesGrouping
    {
        private Grouping _grouping = null;
        List<SortBy> _sorting = new List<SortBy>();
        private Expression _label = null;

        public DynamicSeries(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            base.ParseAttribute(attr);
            switch (attr.Name.ToLower())
            {
                case "grouping":
                    _grouping = new Grouping(attr, this);
                    break;
                case "sorting":
                    foreach (XmlNode child in attr.ChildNodes)
                        _sorting.Add(new SortBy(child, this));
                    break;
                case "label":
                    _label = new Expression(attr, this);
                    break;
                default:
                    break;
            }
        }

        public override SeriesList GetSeriesList(Rdl.Runtime.Context context, Series parentSeries)
        {
            List<Series> result = new List<Series>();
            Rdl.Runtime.Context ctxt = context.GetChildContext(null, null, _grouping, _sorting);

            while (ctxt.GroupIndex < ctxt.GroupCount)
            {
                string value;
                if (_label != null && !_label.Empty)
                    value = _label.ExecAsString(ctxt);
                else
                    value = ctxt.CurrentGroupValue;
                result.Add(new Series(ctxt, this, parentSeries, value));

                ctxt.NextGroup();
            }

            return new SeriesList(result);
        }
    }
}
