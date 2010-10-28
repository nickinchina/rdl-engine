using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
{
    class DataRegion : ReportItem
    {
        private bool _keepTogether = false;
        private Expression _noRows = null;
        private string _dataSetName = null;
        private bool _pageBreakAtStart = false;
        private bool _pageBreakAtEnd = false;
        private Filters _filters = null;

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
                default:
                    break;
            }
        }

        public Filters Filters
        {
            get { return _filters; }
        }

        public override void Render(RdlRender.Container box)
        {
            base.Render(box);

            DataSet ds = Report.DataSets[_dataSetName];
            if (ds == null)
                throw new Exception("Invalid dataSetName " + _dataSetName + " in DataRegion " + _name);

            _context = new RdlRuntime.Context(
                ParentContext(ds),
                ds,
                _filters,
                null,
                null);
            box.ContextBase = true;
        }
    }
}
