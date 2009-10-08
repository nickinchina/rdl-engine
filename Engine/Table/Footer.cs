using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Rdl.Engine;

namespace Rdl.Engine.Table
{
    class Footer : TableElement
    {
        private List<Row> _tableRows = new List<Row>();
        private bool _repeatOnNewPage = false;

        public Footer(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "tablerows":
                    foreach(XmlNode child in attr.ChildNodes)
                        _tableRows.Add(new Row(child, this));
                    break;
                case "repeatonnewpage":
                    _repeatOnNewPage = bool.Parse(attr.InnerText);
                    break;
            }
        }

        public bool RepeatOnNewPage
        {
            get { return _repeatOnNewPage; }
        }


        internal override void Render(Rdl.Render.Container box, Rdl.Runtime.Context context)
        {
            foreach (Row row in _tableRows)
                row.Render(box, context);
        }
    }
}
