using System;
using System.Collections.Generic;
using System.Text;

namespace RdlRender
{
    public class BoxElement : Element
    {
        public enum BoxElementTypeEnum
        {
            Positioned,
            List
        };

        private bool _keepTogether = false;
        private Int16 _columns = 1;
        private decimal _columnSpacing = 0;
        // Fixed indicates that the box should remain visible even when the user scrolls the 
        // surrounding text off the screen.
        private bool _fixed = false;
        //private TextElement _toggleItem = null;
        protected List<Element> _childElements = new List<Element>();
        private List<BoxElement> _repeatList = new List<BoxElement>();
        private bool _pageBreakBefore = false;
        private bool _pageBreakAfter = false;
        private BoxElementTypeEnum _boxElementType = BoxElementTypeEnum.Positioned;

        internal BoxElement(BoxElement parent, RdlEngine.ReportElement reportElement, BoxStyle style)
            : base(parent, reportElement, style)
        {
        }


        public BoxElement(BoxElement b) : base(b)
        {
            _keepTogether = b._keepTogether;
            _columns = b._columns;
            _columnSpacing = b._columnSpacing;
            _fixed = b._fixed;
        }

        public BoxElement(BoxElement b, bool copyChildren)
            : this(b)
        {
            if (copyChildren)
            {
                foreach (Element e in b._childElements)
                {
                    Element newElement = null;
                    if (e is BoxElement)
                        newElement = new BoxElement((BoxElement)e, true);
                    if (e is TextElement)
                        newElement = new TextElement((TextElement)e);
                    if (e is ImageElement)
                        newElement = new ImageElement((ImageElement)e);

                    newElement.Parent = this;
                    _childElements.Add(newElement);
                }
            }
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

        public List<BoxElement> RepeatList
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

        public BoxElementTypeEnum BoxElementType
        {
            get { return _boxElementType; }
        }

        //internal TextElement ToggleItem
        //{
        //    get { return _toggleItem; }
        //    set { _toggleItem = value; }
        //}

        // Create a list of BoxElements that are linked to the current box through
        // the ToggleItem attribute.
        public void LinkToggles()
        {
            foreach (Element e in _childElements)
            {
                if (e is TextElement)
                    ((TextElement)e).LinkToggles();
                if (e is BoxElement)
                    ((BoxElement)e).LinkToggles();
            }
        }

        // Find the BoxElement that refers to the specified report element somewhere
        // in the tree below the current box.
        public List<BoxElement> FindReportElements(RdlEngine.ReportElement r)
        {
            List<BoxElement> boxList = new List<BoxElement>();
            RecurseFindReportElements(r, ref boxList);
            return boxList;
        }

        private void RecurseFindReportElements(RdlEngine.ReportElement r, ref List<BoxElement> boxList)
        {
            if (_reportElement == r)
            {
                boxList.Add(this);
                return;
            }
            foreach (Element e in _childElements)
            {
                if (e is BoxElement)
                    ((BoxElement)e).RecurseFindReportElements(r, ref boxList);
            }
        }

        internal BoxElement AddBoxElement(RdlEngine.ReportElement reportElement, RdlEngine.Style style)
        {
            BoxElement child = new BoxElement(this, reportElement, new BoxStyle(style));
            _childElements.Add(child);
            return child;
        }

        internal BoxElement AddListBoxElement(RdlEngine.ReportElement, RdlEngine.Style style)
        {
            BoxElement child = new BoxElement(this, reportElement, new BoxStyle(style));
            child._boxElementType = BoxElementTypeEnum.List;
            _childElements.Add(child);
            return child;
        }

        //public BoxElement AddBoxElement(int styleIndex)
        //{
        //    BoxElement child = new BoxElement(this, null);
        //    _styleIndex = styleIndex;
        //    _childElements.Add(child);
        //    return child;
        //}

        internal TextElement AddTextElement(RdlEngine.ReportElement reportElement, string name, string text, RdlEngine.Style style)
        {
            TextElement child = new TextElement(this, reportElement, name, text, new TextStyle(style));
            _childElements.Add(child);
            return child;
        }
    }
}
