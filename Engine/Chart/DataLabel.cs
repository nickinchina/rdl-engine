using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine.Chart
{
    class DataLabel : ChartElement
    {
        public enum PositionEnum
        {
            Auto,
            Top,
            TopLeft,
            TopRight,
            Left,
            Center,
            Right,
            Bottom,
            BottomRight,
            BottomLeft
        };

        private Expression _value = null;
        private bool _visible = false;
        private PositionEnum _position = PositionEnum.Auto;
        private int _rotation = 0;

        public DataLabel(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            base.ParseAttribute(attr);
            switch (attr.Name.ToLower())
            {
                case "style":
                    _style = new Style(attr, this);
                    break;
                case "value":
                    _value = new Expression(attr, this);
                    break;
                case "visible":
                    _visible = bool.Parse(attr.InnerText);
                    break;
                case "position":
                    _position = (PositionEnum)Enum.Parse(typeof(PositionEnum), attr.InnerText, true);
                    break;
                case "rotation":
                    _rotation = int.Parse(attr.InnerText);
                    break;
                default:
                    break;
            }
        }
    }
}
