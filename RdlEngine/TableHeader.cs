using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
{
    class TableHeader : ReportElement
    {
        private List<TableRow> _tableRows = new List<TableRow>();
        private bool _repeatOnNewPage = false;
        private bool _fixedHeader = false;

        public TableHeader(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "tablerows":
                    foreach(XmlNode child in attr.ChildNodes)
                        _tableRows.Add(new TableRow(child, this));
                    break;
                case "repeatonnewpage":
                    _repeatOnNewPage = bool.Parse(attr.InnerText);
                    break;
                case "fixedheader":
                    _fixedHeader = bool.Parse(attr.InnerText);
                    break;
                default:
                    break;
            }
        }

        public bool RepeatOnNewPage
        {
            get { return _repeatOnNewPage; }
        }

        public override void Render(RdlRender.Container box)
        {
            base.Render(box);
            if (box != null)
            {
                box.Fixed = _fixedHeader;
            }

            foreach (TableRow row in _tableRows)
                row.Render(box);
        }
    }
}
