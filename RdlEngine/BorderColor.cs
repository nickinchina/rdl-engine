using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
{
    public class BorderColor : ReportElement
    {
        private Expression _default = null;
        private Expression _left = null;
        private Expression _right = null;
        private Expression _top = null;
        private Expression _bottom = null;

        public BorderColor(XmlNode node, ReportElement parent)
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

        public string Default
        {
            get { return (_default == null) ? "black" : _default.ExecAsString(); }
        }

        public string Left
        {
            get { return (_left == null) ? Default : _left.ExecAsString(); }
        }

        public string Right
        {
            get { return (_right == null) ? Default : _right.ExecAsString(); }
        }

        public string Top
        {
            get { return (_top == null) ? Default : _top.ExecAsString(); }
        }

        public string Bottom
        {
            get { return (_bottom == null) ? Default : _bottom.ExecAsString(); }
        }
    }
}
