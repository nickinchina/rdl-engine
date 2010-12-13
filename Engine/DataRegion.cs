using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine
{
    class DataRegion : ReportItem
    {
        private bool _keepTogether = false;
        private Expression _noRows = null;
        private string _dataSetName = null;
        private bool _pageBreakAtStart = false;
        private bool _pageBreakAtEnd = false;
        private Filters _filters = null;
        protected Rdl.Runtime.Context _context = null;

        public DataRegion(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            base.ParseAttribute(attr);
            switch (attr.Name.ToLower())
            {
                case "keeptogether":
                    _keepTogether = bool.Parse(attr.InnerText);
                    break;
                case "norows":
                    _noRows = new Expression(attr, this);
                    break;
                case "datasetname":
                    _dataSetName = attr.InnerText;
                    break;
                case "pagebreakatstart":
                    _pageBreakAtStart = bool.Parse(attr.InnerText);
                    break;
                case "pagebreakatend":
                    _pageBreakAtEnd = bool.Parse(attr.InnerText);
                    break;
                case "filters":
                    _filters = new Filters(attr, this);
                    break;
                default:
                    break;
            }
        }

        public Filters Filters
        {
            get { return _filters; }
        }

        protected override void Render1(Rdl.Render.Container parentBox, Rdl.Runtime.Context context, bool visible)
        {
            DataSet ds = null;
            if (_dataSetName != null)
                ds = Report.DataSets[_dataSetName];
            else if (context.DataSet != null)
                ds = context.DataSet;
            else if (Report.DataSets.Count == 1)
                ds = Report.DataSets.FirstDataSet;
            else
                throw new Exception("Missing data set name in dataregion " + this.Name);
                
            if (ds == null)
                throw new Exception("Invalid dataSetName " + _dataSetName + " in DataRegion " + _name);

            _context = new Rdl.Runtime.Context(
                context.FindContextByDS(ds),
                ds,
                _filters,
                null,
                null);
            _context.ReportItemName = Name;
            parentBox.ContextBase = true;
        }

        protected override void Render2(Rdl.Runtime.Context context)
        {
        }

        public Rdl.Runtime.Context Context
        {
            get { return _context; }
        }
    }
}
