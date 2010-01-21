using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
{
    class Body : ReportElement
    {
        private ReportItems _reportItems = null;
        private Size _height = null;
        private Int16 _columns = 1;
        private Size _columnSpacing = new Size("0.5in");

        public Body(XmlNode node, ReportElement parent)
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
                case "height":
                    _height = new Size(attr.InnerText);
                    break;
                case "columns":
                    _columns = Int16.Parse(attr.InnerText);
                    break;
                case "columnspacing":
                    _columnSpacing = new Size(attr.InnerText);
                    break;
                case "style":
                    _style = new Style(attr, this);
                    break;
                default:
                    break;
            }
        }

        public override void Render(RdlRender.Container box)
        {
            if (box != null)
            {
                box.Columns = _columns;
                box.ColumnSpacing = _columnSpacing.points;

                if (_columns > 1)
                {
                    box.Width = (box.Width / _columns) -
                        (_columnSpacing.points * (_columns - 1));
                }
            }

            _reportItems.Render(box);
        }
    }
}
