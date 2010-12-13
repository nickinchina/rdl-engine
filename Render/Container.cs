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

namespace Rdl.Render
{
    // A Container is a box for holding other elements.
    public class Container : Element
    {
        private Int16 _columns = 1;
        private decimal _columnSpacing = 0;
        protected List<Element> _childElements = new List<Element>();

        internal Container(Container parent, Rdl.Engine.ReportElement reportElement, BoxStyle style)
            : base(parent, reportElement, style)
        {
        }

        public Container(Container b) : base(b)
        {
            _columns = b._columns;
            _columnSpacing = b._columnSpacing;
        }

        public Container(Container b, bool copyChildren)
            : this(b)
        {
            if (copyChildren)
            {
                foreach (Element e in b._childElements)
                {
                    Element newElement = null;
                    if (e is FixedContainer)
                        newElement = new FixedContainer((FixedContainer)e, true);
                    if (e is FlowContainer)
                        newElement = new FlowContainer((FlowContainer)e, true);
                    if (e is TextElement)
                        newElement = new TextElement((TextElement)e);
                    if (e is ImageElement)
                        newElement = new ImageElement((ImageElement)e);
                    if (e is ChartElement)
                        newElement = new ChartElement((ChartElement)e);

                    newElement.Parent = this;
                    _childElements.Add(newElement);
                }
            }
        }

        public List<Element> Children
        {
            get { return _childElements; }
        }

        public Int16 Columns
        {
            set { _columns = value; }
            get { return _columns; }
        }

        public decimal ColumnSpacing
        {
            set { _columnSpacing = value; }
            get { return _columnSpacing; }
        }

        // Find the Container that refers to the specified report element somewhere
        // in the tree below the current box.
        public List<Container> FindReportElements(Rdl.Engine.ReportElement r)
        {
            Container parent = this;

            List<Container> boxList = new List<Container>();
            while (parent != null)
            {
                if (parent.ContextBase)
                {
                    RecurseFindReportElements(r, boxList);
                    if (boxList.Count > 0)
                        return boxList;
                }

                parent = parent.Parent;
            }
            return null;
        }

        private void RecurseFindReportElements(Rdl.Engine.ReportElement r, List<Container> boxList)
        {
            if (_reportElement == r)
            {
                boxList.Add(this);
                return;
            }
            foreach (Element e in _childElements)
            {
                if (e is Container)
                    ((Container)e).RecurseFindReportElements(r, boxList);
            }
        }

        internal FixedContainer AddFixedContainer(Rdl.Engine.ReportElement reportElement, Rdl.Engine.Style style, Rdl.Runtime.Context context)
        {
            FixedContainer child = new FixedContainer(this, reportElement, new BoxStyle(style, context));
            _childElements.Add(child);
            if (style != null && style.BackgroundImage != null)
                _imageIndex = Render.AddImage(style.BackgroundImage, context);
            return child;
        }

        internal FlowContainer AddFlowContainer(Rdl.Engine.ReportElement reportElement, Rdl.Engine.Style style, Rdl.Runtime.Context context, Rdl.Render.FlowContainer.FlowDirectionEnum direction)
        {
            FlowContainer child = new FlowContainer(this, reportElement, new BoxStyle(style, context));
            child.FlowDirection = direction;
            if (style != null && style.BackgroundImage != null)
                _imageIndex = Render.AddImage(style.BackgroundImage, context);
            _childElements.Add(child);
            return child;
        }

        internal ChartElement AddChartElement(Rdl.Engine.ReportElement reportElement, Rdl.Engine.Style style, Rdl.Runtime.Context context)
        {
            ChartElement child = new ChartElement(this, reportElement, new BoxStyle(style, context), context);
            if (style != null && style.BackgroundImage != null)
                _imageIndex = Render.AddImage(style.BackgroundImage, context);
            _childElements.Add(child);
            return child;
        }

        internal Table.Container AddTableContainer(Rdl.Engine.ReportElement reportElement, Rdl.Engine.Style style, Rdl.Runtime.Context context)
        {
            Table.Container child = new Table.Container(this, reportElement, new BoxStyle(style, context));
            if (style != null && style.BackgroundImage != null)
                _imageIndex = Render.AddImage(style.BackgroundImage, context);
            _childElements.Add(child);
            return child;
        }

        internal TextElement AddTextElement(Rdl.Engine.ReportElement reportElement, string name, string text, Rdl.Engine.Style style, Rdl.Runtime.Context context)
        {
            TextElement child = new TextElement(this, reportElement, name, text, new TextStyle(style, context), context);
            if (style != null && style.BackgroundImage != null)
                _imageIndex = Render.AddImage(style.BackgroundImage, context);
            _childElements.Add(child);
            return child;
        }

        internal ImageElement AddImageElement(Rdl.Engine.ReportElement reportElement,
            Rdl.Engine.Style style, Rdl.Runtime.Context context)
        {
            ImageElement child = new ImageElement(this, reportElement, new BoxStyle(style, context));
            _childElements.Add(child);
            return child;
        }

        internal override void SetParentSizes(bool ignoreVisibility)
        {
            base.SetParentSizes(ignoreVisibility);

            if (!ignoreVisibility && !IsVisible)
                return;

            foreach (Element child in _childElements)
            {
                child.SetParentSizes(ignoreVisibility);

                _renderedHeight = Math.Max(_renderedHeight, child.Top + child.Height);
                _renderedWidth = Math.Max(_renderedWidth, child.Left + child.Width);
            }
        }

        internal override void SetChildSizes(bool ignoreVisibility)
        {
            base.SetChildSizes(ignoreVisibility);

            if (!ignoreVisibility && !IsVisible)
                return;

            foreach (Element child in _childElements)
                child.SetChildSizes(ignoreVisibility);
        }

        //public override decimal TotalHeight(bool ignoreVisibility)
        //{
        //    decimal height = 0;

        //    foreach (Element child in _childElements)
        //        height = Math.Max(height, child.Top + child.TotalHeight(ignoreVisibility));
        //    return Math.Max(height, _height);
        //}

        //public override decimal TotalWidth(bool ignoreVisibility)
        //{
        //    decimal width = 0;

        //    foreach (Element child in _childElements)
        //        width = Math.Max(width, child.Left + child.TotalWidth(ignoreVisibility));
        //    return Math.Max(width, _width);
        //}
    }
}
