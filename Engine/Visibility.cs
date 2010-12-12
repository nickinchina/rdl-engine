using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine
{
    public class Visibility : ReportElement
    {
        private Expression _hidden = null;
        private string _toggleItem = null;
        public static Visibility Visible = new Visibility("false");

        public Visibility(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        private Visibility( string hidden ) 
            : base( null, null)
        {
            _hidden = new Expression(hidden, null);
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "hidden":
                    _hidden = new Expression(attr, this);
                    break;
                case "toggleitem":
                    _toggleItem = attr.InnerText;
                    break;
                default:
                    break;
            }
        }

        public bool IsHidden(Rdl.Runtime.Context context)
        {
            return _hidden.ExecAsBoolean(context);
        }

        public string ToggleItem
        {
            get { return _toggleItem; }
        }
    }
}
