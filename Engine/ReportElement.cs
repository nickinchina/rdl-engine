/*-----------------------------------------------------------------------------------
This file is part of the SawikiSoft RDL Engine.
The SawikiSoft RDL Engine is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

The SawikiSoft RDL Engine is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
-----------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine
{
    public abstract class ReportElement
    {
        private ReportElement _parent = null;
        private Report _report = null;
        protected bool _container = false;
        protected bool _cell = false;
        protected Style _style = null;
        //internal Rdl.Runtime.Context _context = null;
        private List<ReportElement> _childList = new List<ReportElement>();

        protected ReportElement(XmlNode node, ReportElement parent)
        {
            // Set the Parent and Report pointers
            _parent = parent;
            if (parent != null)
            {
                if (parent is Report)
                    _report = (Report)parent;
                else
                    _report = parent.Report;
                _parent._childList.Add(this);
            }
            if (this is Report)
                _report = this as Report;

            ParseAttributes(node);
        }

        protected void ParseAttributes(XmlNode node)
        {
            // Parse all of the attributes and child nodes.
            if (node != null)
            {
                if (node.Attributes != null)
                    foreach (XmlAttribute attr in node.Attributes)
                        ParseAttribute(attr);

                if (node.ChildNodes != null)
                    foreach (XmlNode child in node.ChildNodes)
                        ParseAttribute(child);
            }
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

        internal Table.Table FindTable(ReportElement elmt)
        {
            if (elmt is Table.Table)
                return (Table.Table)elmt;
            else if (elmt.Parent != null)
                return FindTable(elmt.Parent);
            return null;
        }

        internal Matrix.Matrix FindMatrix(ReportElement elmt)
        {
            if (elmt is Matrix.Matrix)
                return (Matrix.Matrix)elmt;
            else if (elmt.Parent != null)
                return FindMatrix(elmt.Parent);
            return null;
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

        internal virtual void Render(Rdl.Render.Container box, Rdl.Runtime.Context context)
        {
            throw new Exception("Not implemented");
        }

        //internal Rdl.Runtime.Context Context
        //{
        //    get
        //    {
        //        // Find the next context up in the report tree.
        //        for (ReportElement parent = this; parent != null; parent = parent._parent)
        //        {
        //            if (parent._context != null)
        //                return parent._context;
        //        }
        //        return null;
        //    }
        //}

        //internal Rdl.Runtime.Context ParentContext(DataSet ds)
        //{
        //    // Find the next context up in the report tree.
        //    for (ReportElement parent = _parent; parent != null; parent = parent._parent)
        //        if (parent._context != null)
        //        {
        //            if (ds == null)
        //                return parent._context;

        //            // Look for the next context up that matches this dataset or is
        //            // a parent of this dataset.
        //            if (parent._context.DataSet == ds ||
        //                Report.DataSets.systemDs.Relations.Contains(ds.Name + "to" + parent._context.DataSet.Name))
        //                return (parent._context);
        //        }
        //    return null;
        //}

        public bool IsInCell
        {
            get
            {
                ReportElement parent = _parent;
                while (parent != null && !parent._container)
                    parent = parent._parent;
                if (parent != null)
                    return parent._cell;
                return false;
            }
        }

        internal TextBox FindToggleItem(Visibility v)
        {
            if (v != null && v.ToggleItem != null)
            {
                ReportItem toggleItem = Report.ReportItems[v.ToggleItem];
                if (toggleItem != null)
                {
                    if (toggleItem is TextBox)
                        return (TextBox)toggleItem;
                    else
                        throw new Exception("Toggle items are only allowed to refer to text boxes\n");
                }
                else
                    throw new Exception("Toggle item " + v.ToggleItem + " not found");
            }
            return null;
        }

    }
}
