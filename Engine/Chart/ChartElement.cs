using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine.Chart
{
    class ChartElement : ReportElement
    {
        public ChartElement(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
        }

        internal override void Render(Rdl.Render.Container box, Rdl.Runtime.Context context)
        {
        }
    }
}
