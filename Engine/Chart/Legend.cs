using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine.Chart
{
    class Legend : ChartElement
    {
        public enum PositionEnum
        {
            TopLeft,
            TopCenter,
            TopRight,
            LeftTop,
            LeftCenter,
            LeftBottom,
            RightTop,
            RightCenter,
            RightBottom,
            BottomRight,
            BottomCenter,
            BottomLeft
        };

        public enum LayoutEnum
        {
            Column,
            Row,
            Table
        };

        private bool _visible = false;
        private PositionEnum _position = PositionEnum.RightTop;
        private LayoutEnum _layout = LayoutEnum.Column;
        private bool _insidePlotArea = false;

        public Legend(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            base.ParseAttribute(attr);
            switch (attr.Name.ToLower())
            {
                case "visible":
                    _visible = bool.Parse(attr.InnerText);
                    break;
                case "style":
                    _style = new Style(attr, this);
                    break;
                case "position":
                    _position = (PositionEnum)Enum.Parse(typeof(PositionEnum), attr.InnerText, true);
                    break;
                case "layout":
                    _layout = (LayoutEnum)Enum.Parse(typeof(LayoutEnum), attr.InnerText, true);
                    break;
                case "insideplotarea":
                    _insidePlotArea = bool.Parse(attr.InnerText);
                    break;
                default:
                    break;
            }
        }

        public bool Visible
        {
            get { return _visible; }
        }

        public PositionEnum Position
        {
            get { return _position; }
        }

        public LayoutEnum Layout
        {
            get { return _layout; }
        }

        public bool InsidePlotArea
        {
            get { return _insidePlotArea; }
        }
    }
}
