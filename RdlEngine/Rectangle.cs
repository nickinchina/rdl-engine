using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
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

        public override void Render(RdlRender.Container box)
        {
            base.Render(box);

            base.Render(box);

            bool visible = true;
            if (_visibility != null && _visibility.IsHidden && _visibility.ToggleItem == null)
                visible = false;
            if (box != null && visible)
            {
                _box = box.AddFixedContainer(this, Style);
                _box.PageBreakBefore = _pageBreakAtStart;
                _box.PageBreakAfter = _pageBreakAtEnd;
                _box.Name = "Rectangle";
                if (IsInCell)
                {
                    _box.Width = box.Width;
                    _box.MatchParentHeight = true;
                }
                else
                {
                    _box.Top = _top.points;
                    _box.Left = _left.points;
                    _box.Width = (_width == null) ? box.Width : _width.points;
                    _box.Height = _height.points;
                }
            }

            _reportItems.Render(_box);
        }
    }
}
