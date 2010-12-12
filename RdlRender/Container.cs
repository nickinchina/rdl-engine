using System;
using System.Collections.Generic;
using System.Text;

namespace RdlRender
{
    // A Container is a box for holding other elements.
    public class Container : Element
    {
        private bool _keepTogether = false;
        private Int16 _columns = 1;
        private decimal _columnSpacing = 0;
        // Fixed indicates that the box should remain visible even when the user scrolls the 
        // surrounding text off the screen.
        private bool _fixed = false;
        protected List<Element> _childElements = new List<Element>();
        //RepeatList lists which child elements should be repeated
        // if the container spans more than one page.
        private List<Container> _repeatList = new List<Container>();
        private bool _pageBreakBefore = false;
        private bool _pageBreakAfter = false;
        private TextElement _toggle = null;

        internal Container(Container parent, RdlEngine.ReportElement reportElement, BoxStyle style)
            : base(parent, reportElement, style)
        {
        }


        public Container(Container b) : base(b)
        {
            _keepTogether = b._keepTogether;
            _columns = b._columns;
            _columnSpacing = b._columnSpacing;
            _fixed = b._fixed;
            _toggle = b._toggle;
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

                    newElement.Parent = this;
                    _childElements.Add(newElement);
                }
            }
        }

        public Container Copy(bool copyChildren)
        {
            if (this is FixedContainer)
                return new FixedContainer(this, copyChildren);
            if (this is FlowContainer)
                return new FlowContainer(this, copyChildren);
            return null;
        }

        public List<Element> Children
        {
            get { return _childElements; }
        }

        public bool KeepTogether
        {
            set { _keepTogether = value; }
            get { return _keepTogether; }
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

        public List<Container> RepeatList
        {
            get { return _repeatList; }
        }

        public bool Fixed
        {
            get { return _fixed; }
            set { _fixed = value; }
        }

        public bool PageBreakBefore
        {
            set { _pageBreakBefore = value; }
            get { return _pageBreakBefore; }
        }

        public bool PageBreakAfter
        {
            set { _pageBreakAfter = value; }
            get { return _pageBreakAfter; }
        }

        public TextElement Toggle
        {
            get { return _toggle; }
            set { _toggle = value; }
        }

        // Create a list of Containers that are linked to the current box through
        // the ToggleItem attribute.
        public void LinkToggles()
        {
            foreach (Element e in _childElements)
            {
                if (e is TextElement)
                    ((TextElement)e).LinkToggles();
                if (e is Container)
                    ((Container)e).LinkToggles();
            }
        }

        // Find the Container that refers to the specified report element somewhere
        // in the tree below the current box.
        public List<Container> FindReportElements(RdlEngine.ReportElement r)
        {
            List<Container> boxList = new List<Container>();
            RecurseFindReportElements(r, ref boxList);
            return boxList;
        }

        private void RecurseFindReportElements(RdlEngine.ReportElement r, ref List<Container> boxList)
        {
            if (_reportElement == r)
            {
                boxList.Add(this);
                return;
            }
            foreach (Element e in _childElements)
            {
                if (e is Container)
                    ((Container)e).RecurseFindReportElements(r, ref boxList);
            }
        }

        internal FixedContainer AddFixedContainer(RdlEngine.ReportElement reportElement, RdlEngine.Style style)
        {
            FixedContainer child = new FixedContainer(this, reportElement, new BoxStyle(style));
            _childElements.Add(child);
            return child;
        }

        internal FlowContainer AddFlowContainer(RdlEngine.ReportElement reportElement, RdlEngine.Style style)
        {
            FlowContainer child = new FlowContainer(this, reportElement, new BoxStyle(style));
            _childElements.Add(child);
            return child;
        }

        internal TextElement AddTextElement(RdlEngine.ReportElement reportElement, string name, string text, RdlEngine.Style style)
        {
            TextElement child = new TextElement(this, reportElement, name, text, new TextStyle(style));
            _childElements.Add(child);
            return child;
        }
    }
}
