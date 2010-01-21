using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
{
    public class BorderStyle : ReportElement
    {
        public enum BorderStyleEnum
        {
            None,
            Dotted,
            Dashed,
            Solid,
            Double,
            Groove,
            Ridge,
            Inset,
            WindowInset,
            Outset
        };

        private Expression _default = null;
        private Expression _left = null;
        private Expression _right = null;
        private Expression _top = null;
        private Expression _bottom = null;

        public BorderStyle(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "default":
                    _default = new Expression(attr, this);
                    break;
                case "left":
                    _left = new Expression(attr, this);
                    break;
                case "right":
                    _right = new Expression(attr, this);
                    break;
                case "top":
                    _top = new Expression(attr, this);
                    break;
                case "bottom":
                    _bottom = new Expression(attr, this);
                    break;
                default:
                    break;
            }
        }

        public BorderStyleEnum Default
        {
            get { return (_default == null) ? BorderStyleEnum.None : (BorderStyleEnum)Enum.Parse(typeof(BorderStyleEnum), _default.ExecAsString(),true); }
        }

        public BorderStyleEnum Left
        {
            get { return (_left == null) ? Default : (BorderStyleEnum)Enum.Parse(typeof(BorderStyleEnum), _left.ExecAsString(), true); }
        }

        public BorderStyleEnum Right
        {
            get { return (_right == null) ? Default : (BorderStyleEnum)Enum.Parse(typeof(BorderStyleEnum), _right.ExecAsString(), true); }
        }

        public BorderStyleEnum Top
        {
            get { return (_top == null) ? Default : (BorderStyleEnum)Enum.Parse(typeof(BorderStyleEnum), _top.ExecAsString(), true); }
        }

        public BorderStyleEnum Bottom
        {
            get { return (_bottom == null) ? Default : (BorderStyleEnum)Enum.Parse(typeof(BorderStyleEnum), _bottom.ExecAsString(), true); }
        }
    }
}
