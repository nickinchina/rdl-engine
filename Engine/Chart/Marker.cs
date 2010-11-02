using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine.Chart
{
    class Marker : ChartElement
    {
        public enum TypeEnum
        {
            None,
            Square,
            Circle,
            Diamond,
            Triangle,
            Cross,
            Auto
        };

        private TypeEnum _type = TypeEnum.None;
        private Size _size = Size.ZeroSize;

        public Marker(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            base.ParseAttribute(attr);
            switch (attr.Name.ToLower())
            {
                case "type":
                    _type = (TypeEnum)Enum.Parse(typeof(TypeEnum), attr.InnerText, true);
                    break;
                case "size":
                    _size = new Size(attr.InnerText);
                    break;
                case "style":
                    _style = new Style(attr, this);
                    break;
                default:
                    break;
            }
        }
    }
}
