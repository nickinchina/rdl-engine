using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine
{
    class ToggleImage : ReportElement
    {
        private Expression _initialState = null;

        public ToggleImage(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "initialstate":
                    _initialState = new Expression(attr, this);
                    break;
                default:
                    break;
            }
        }

        public bool InitialState(Rdl.Runtime.Context context)
        {
            return _initialState.ExecAsBoolean(context);
        }
    }
}
