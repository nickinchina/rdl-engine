using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
{
    public abstract class ReportElement
    {
        private ReportElement _parent = null;
        private Report _report = null;
        protected bool _container = false;
        protected bool _cell = false;
        protected Style _style = null;
        internal RdlRuntime.Context _context = null;
        private List<ReportElement> _childList = new List<ReportElement>();

        protected ReportElement(XmlNode node, ReportElement parent)
        {
            _parent = parent;
            if (parent != null)
            {
                if (parent is Report)
                    _report = (Report)parent;
                else
                    _report = parent.Report;
                _parent._childList.Add(this);
            }

            if (node != null)
            {
                foreach (XmlAttribute attr in node.Attributes)
                    ParseAttribute(attr);

                foreach (XmlNode child in node.ChildNodes)
                    ParseAttribute(child);
            }
        }

        internal virtual void Parse2()
        {
            foreach (ReportElement r in _childList)
                r.Parse2();
        }

        public Report Report
        {
            get { return _report; }
        }

        public ReportElement Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public Style Style
        {
            get
            {
                if (_style != null)
                    return _style;
                else
                    return Style.DefaultStyle;
                //{
                //    if (_parent != null)
                //        return _parent.Style;
                //    //_style = new Style(null, this);
                //    return null;
                //}
            }
        }

        protected abstract void ParseAttribute(XmlNode attr);

        public virtual void Render(RdlRender.Container box)
        {
        }

        internal RdlRuntime.Context Context
        {
            get
            {
                // Find the next context up in the report tree.
                for (ReportElement parent = this; parent != null; parent = parent._parent)
                {
                    if (parent._context != null)
                        return parent._context;
                }
                return null;
            }
        }

        internal RdlRuntime.Context ParentContext(DataSet ds)
        {
            // Find the next context up in the report tree.
            for (ReportElement parent = _parent; parent != null; parent = parent._parent)
                if (parent._context != null)
                {
                    if (ds == null)
                        return parent._context;

                    // Look for the next context up that matches this dataset or is
                    // a parent of this dataset.
                    if (parent._context.DataSet == ds ||
                        Report.DataSets.systemDs.Relations.Contains(ds.Name + "to" + parent._context.DataSet.Name))
                        return (parent._context);
                }
            return null;
        }

        public bool IsInCell
        {
            get
            {
                if (!_container)
                    return (_parent.IsInCell);
                return (_cell);
            }
        }
    }
}
