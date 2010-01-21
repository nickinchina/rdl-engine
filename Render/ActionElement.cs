using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Render
{
    public class ActionElement : Element
    {
        public class ActionParameter
        {
            public string Name = string.Empty;
            public object Value = null;

            public ActionParameter(string name, object value)
            {
                Name = name;
                Value = value;
            }
        }

        private string _hyperLink;
        private string _bookmarkLink;
        private string _drillThroughReportName;
        private List<ActionParameter> _drillThroughParameterList = new List<ActionParameter>();

        public ActionElement(Container parent, 
            Rdl.Engine.ReportElement reportItem, 
            BoxStyle style
            )
            : base(parent, reportItem, style)
        {
        }

        public ActionElement(Element e)
            : base(e)
        {
        }

        public string BookmarkLink
        {
            get { return _bookmarkLink; }
            set { _bookmarkLink = value; }
        }

        public string Hyperlink
        {
            get { return _hyperLink; }
            set { _hyperLink = value; }
        }

        public string DrillThroughReportName
        {
            get { return _drillThroughReportName; }
            set { _drillThroughReportName = value; }
        }

        public List<ActionParameter> DrillThroughParameterList
        {
            get { return _drillThroughParameterList; }
        }
    }
}
