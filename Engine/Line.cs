using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine
{
    class Line : ReportItem
    {
        public Line(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void Render1(Rdl.Render.Container parentBox, Rdl.Runtime.Context context, bool visible)
        {
            if (visible)
                _box = parentBox.AddFixedContainer(this, Style, context);
        }

        protected override void Render2(Rdl.Runtime.Context context)
        {
        }
    }
}
