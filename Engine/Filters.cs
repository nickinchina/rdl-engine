using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine
{
    class Filters : ReportElement
    {
        private List<Filter> _filters = new List<Filter>();
        private Int32 _filterIndex = 0;
        public List<System.Data.DataRow> DataRows = new List<System.Data.DataRow>();

        public Filters(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "filter":
                    _filters.Add(new Filter(attr, this));
                    break;
                default:
                    break;
            }
        }

        public Int32 FilterIndex
        {
            get { return _filterIndex; }
        }

        private bool shouldReinitialize()
        {
            for (ReportElement parent = Parent; parent != null; parent = parent.Parent)
            {
                if (parent is DataRegion)
                    if (((DataRegion)parent).Filters.FilterIndex > _filterIndex)
                        return true;
            }
            return false;
        }

        private void Initialize()
        {
            _filterIndex = Report.FilterIndex;
        }

        public IEnumerator GetEnumerator()
        {
            return _filters.GetEnumerator();
        }

        public Filter GroupFilter
        {
            get
            {
                foreach (Filter f in _filters)
                    if (f.Operator == Filter.FilterOperator.TopN ||
                        f.Operator == Filter.FilterOperator.TopPercent ||
                        f.Operator == Filter.FilterOperator.BottomN ||
                        f.Operator == Filter.FilterOperator.BottomPercent)
                        return f;
                return null;
            }
        }
    }
}
