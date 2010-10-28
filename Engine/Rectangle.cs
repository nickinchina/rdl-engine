using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine
{
    class Rectangle : ReportItem
    {
        bool _pageBreakAtStart = false;
        bool _pageBreakAtEnd = false;
        ReportItems _reportItems = null;

        public Rectangle(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
            _container = true;
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            base.ParseAttribute(attr);
            switch (attr.Name.ToLower())
            {
                case "reportitems":
                    _reportItems = new ReportItems(attr, this);
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

        protected override void Render1(Rdl.Render.Container box, Rdl.Runtime.Context context, bool visible)
        {
            if (visible)
            {
                _box = box.AddFixedContainer(this, Style, context);
                _box.PageBreakBefore = _pageBreakAtStart;
                _box.PageBreakAfter = _pageBreakAtEnd;
                _box.KeepTogether = true;
                _box.Name = "Rectangle";
            }
        }

        protected override void Render2(Rdl.Runtime.Context context)
        {
            if (_reportItems != null)
                _reportItems.Render((Rdl.Render.Container)_box, context);
        }
    }
}
