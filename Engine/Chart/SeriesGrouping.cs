using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Drawing;

namespace Rdl.Engine.Chart
{
    class SeriesGrouping : ChartElement
    {
        protected SeriesGrouping _nextGrouping = null;
        protected Dictionary<string, Color> _colors = new Dictionary<string, Color>();

        public static SeriesGrouping GetSeriesGrouping(XmlNode node, ReportElement parent)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(node.OwnerDocument.NameTable);
            ns.AddNamespace("rdl", parent.Report.XmlNameSpace);

            if (node.SelectSingleNode("rdl:DynamicSeries", ns) != null)
                return new DynamicSeries(node, parent);
            else
                return new StaticSeries(node, parent);
        }


        public SeriesGrouping(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            base.ParseAttribute(attr);
            switch (attr.Name.ToLower())
            {
                case "style":
                    _style = new Style(attr, this);
                    break;
                case "dynamicseries":
                case "staticseries":
                    foreach (XmlNode child in attr.ChildNodes)
                        ParseAttribute(child);
                    break;
                default:
                    break;
            }
        }

        public SeriesGrouping NextGrouping
        {
            get { return _nextGrouping; }
            set { _nextGrouping = value; }
        }

        public virtual SeriesList GetSeriesList(Rdl.Runtime.Context context, Series parentSeries)
        {
            return null;
        }
    }

    class Series
    {
        private string _value;
        private SeriesList _seriesList = null;
        private Rdl.Runtime.Context _context = null;
        private Rdl.Runtime.ContextState _contextState = null;
        private Series _parentSeries = null;
        private int _seriesIndex = 0;

        public Series(Rdl.Runtime.Context context, SeriesGrouping grouping, Series parentSeries, string value)
        {
            _context = context;
            _contextState = context.ContextState;
            _parentSeries = parentSeries;
            _value = value;
            if (grouping.NextGrouping != null)
                _seriesList = grouping.NextGrouping.GetSeriesList(context, this);
        }

        public Series(Rdl.Runtime.Context context, SeriesGrouping grouping, Series parentSeries, string value, int index)
            : this(context, grouping, parentSeries, value)
        {
            _seriesIndex = index;
        }


        public SeriesList SeriesList
        {
            get { return _seriesList; }
        }

        public string Value
        {
            get { return _value; }
        }

        public string CombinedValue
        {
            get
            {
                if (_parentSeries != null)
                    return _parentSeries.CombinedValue + " - " + Value;
                else
                    return Value;
            }
        }

        public Rdl.Runtime.Context Context
        {
            get
            {
                _context.ContextState = _contextState;
                return _context;
            }
        }

        public virtual int SeriesIndex
        {
            get
            {
                if (_seriesIndex > 0 || _parentSeries == null)
                    return _seriesIndex;
                else
                    return _parentSeries.SeriesIndex;
            }
        }
    }

    class SeriesList
    {
        private List<Series> _values = null;

        public SeriesList(List<Series> values)
        {
            _values = values;
        }

        public IEnumerator<Series> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        public int Count
        {
            get { return _values.Count; }
        }

        public SeriesList LeafSeriesList
        {
            get
            {
                List<Series> listSeriesList = new List<Series>();
                BuildLeafSeriesList(this, listSeriesList);
                return new SeriesList(listSeriesList);
            }
        }

        private void BuildLeafSeriesList(SeriesList seriesList, List<Series> leafCategories)
        {
            foreach (Series ser in seriesList)
                if (ser.SeriesList == null)
                    leafCategories.Add(ser);
                else
                    BuildLeafSeriesList(ser.SeriesList, leafCategories);
        }
    }

}
