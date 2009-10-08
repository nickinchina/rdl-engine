using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
{
    class ReportItems : ReportElement
    {
        private List<ReportItem> _reportItems = new List<ReportItem>();

        public ReportItems(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            ReportItem ri = ReportItem.NewReportItem(attr, this);
            if (ri != null)
                _reportItems.Add(ri);
        }

        public override void Render(RdlRender.Container box)
        {
            foreach (ReportItem ri in _reportItems)
            {
                ri.Render(box);
            }

            foreach (ReportItem ri in _reportItems)
            {
                if (ri.RepeatWith != null)
                    foreach (ReportItem ri2 in _reportItems)
                        if (ri2.Name == ri.RepeatWith)
                            ri2.Box.RepeatList.Add(ri.Box);
            }
        }
    }
}
