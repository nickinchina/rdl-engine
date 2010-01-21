using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
{
    public class BorderWidth : ReportElement
    {
        private Expression _default = null;
        private Expression _left = null;
        private Expression _right = null;
        private Expression _top = null;
        private Expression _bottom = null;

        public BorderWidth(XmlNode node, ReportElement parent)
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

        public Size Default
        {
            get { return (_default == null) ? Size.OnePt : new Size(_default.ExecAsString()); }
        }

        public Size Left
        {
            get { return (_left == null) ? Default : new Size(_left.ExecAsString()); }
        }

        public Size Right
        {
            get { return (_right == null) ? Default : new Size(_right.ExecAsString()); }
        }

        public Size Top
        {
            get { return (_top == null) ? Default : new Size(_top.ExecAsString()); }
        }

        public Size Bottom
        {
            get { return (_bottom == null) ? Default : new Size(_bottom.ExecAsString()); }
        }
    }
}
