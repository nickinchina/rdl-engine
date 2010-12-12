using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Render
{
    public abstract class Element
    {
        protected bool _keepTogether = false;
        protected static int _nameIndex = 0;
        protected string _name = "ReportElement_" + (_nameIndex++).ToString();
        internal decimal _top = 0;
        internal decimal _left = 0;
        internal decimal _width = 0;
        internal decimal _height = 0;
        internal decimal _renderedTop = 0;
        internal decimal _renderedLeft = 0;
        internal decimal _renderedWidth = 0;
        internal decimal _renderedHeight = 0;
        public bool MatchParentHeight = false;
        public bool MatchParentWidth = false;
        protected bool _canGrowVertically = true;
        protected bool _canGrowHorizontally = true;
        protected Container _parentElement = null;
        // Fixed indicates that the box should remain visible even when the user scrolls the 
        // surrounding text off the screen.
        protected bool _fixed = false;
        protected bool _pageBreakBefore = false;
        protected bool _pageBreakAfter = false;
        //RepeatList lists which elements should be repeated
        // if the element spans more than one page.
        protected List<Element> _repeatList = new List<Element>();
        internal Rdl.Engine.ReportElement _reportElement;
        protected GenericRender _genericRender;
        protected bool _contextBase = false;
        // The baseStyleList is maintained only in the base element.
        // Each element uses an index into this list to locate its style.
        private int _styleIndex = -1;
        //private int _renderedStyleIndex = -1;
        public int _imageIndex = -1;
        private Toggles _toggles = null;

        internal Element(Container parent, Rdl.Engine.ReportElement reportElement, BoxStyle style)
        {
            _parentElement = parent;
            if (_parentElement != null)
                _genericRender = _parentElement._genericRender;
            _reportElement = reportElement;
            if (style != null && reportElement != null)
                _styleIndex = Render.AddStyle(style);
            _toggles = new Toggles(this);
        }

        public Element(Element e)
        {
            _name = e._name;
            _canGrowHorizontally = e._canGrowHorizontally;
            _canGrowVertically = e._canGrowVertically;
            _reportElement = e._reportElement;
            _styleIndex = e._styleIndex;
            _imageIndex = e._imageIndex;
            _top = e._top;
            _left = e._left;
            _width = e._width;
            _height = e._height;
            _renderedTop = e._renderedTop;
            _renderedLeft = e._renderedLeft;
            _renderedWidth = e._renderedWidth;
            _renderedHeight = e._renderedHeight;
            _keepTogether = e._keepTogether;
            _pageBreakAfter = e._pageBreakAfter;
            _pageBreakBefore = e._pageBreakBefore;
            _fixed = e._fixed;
            _toggles = new Toggles(e._toggles);
            _genericRender = e._genericRender;
        }

        public Container BaseElement()
        {
            if (_parentElement == null)
                return (Container)this;
            else
                return _parentElement.BaseElement();
        }

        public int StyleIndex
        {
            get { return _styleIndex; }
            set { _styleIndex = value; }
        }

        internal void SetGenericRender(GenericRender render)
        {
            _genericRender = render;
        }

        public GenericRender Render
        {
            get { return _genericRender; }
        }

        //public int RenderedStyleIndex
        //{
        //    get { return _renderedStyleIndex; }
        //    set { _renderedStyleIndex = value; }
        //}

        public void SetSizes(bool ignoreVisibility)
        {
            // Sizes are calculated in two passes to allow for the MatchParentHeight / With property
            // The first pass is SetParentSizes to calculate the size of each element
            // based on the sizes of the children
            SetParentSizes(ignoreVisibility);
            // The second pass sets the children to the parent size if the
            // MatchParentHeight property is set.
            SetChildSizes(ignoreVisibility);
        }

        internal virtual void SetParentSizes(bool ignoreVisibility)
        {
            _renderedHeight = 0;
            _renderedWidth = 0;
            _renderedTop = _top;
            _renderedLeft = _left;

            if (!ignoreVisibility && !IsVisible)
                return;

            if (_width != 0)
                _renderedWidth = _width;
            if (_height != 0)
                _renderedHeight = _height;
        }

        internal virtual void SetChildSizes(bool ignoreVisibility)
        {
            if (Parent != null && MatchParentWidth)
            {
                if (Parent is FlowContainer && ((FlowContainer)Parent).FlowDirection == FlowContainer.FlowDirectionEnum.LeftToRight)
                    ((FlowContainer)Parent).FillElement = this;
                else
                    _renderedWidth = Parent._renderedWidth - _left;
            }

            if (Parent != null && MatchParentHeight)
            {
                if (Parent is FlowContainer && ((FlowContainer)Parent).FlowDirection == FlowContainer.FlowDirectionEnum.TopDown)
                    ((FlowContainer)Parent).FillElement = this;
                else
                    _renderedHeight = Parent._renderedHeight - _top;
            }
        }

        public virtual decimal Top
        {
            set 
            {
                _top = value;
                _renderedTop = value;
                //if (_parentElement != null)
                //    if (_parentElement.Height < _height + _top)
                //    {
                //        if (_parentElement._canGrowVertically)
                //            _parentElement.Height = _height + _top;
                //        _height = _parentElement.Height - _top;
                //    }
            }
            get 
            {
                return _renderedTop;
                //return Math.Max(_top, _renderedTop); 
            }
        }

        // Return the top position relative to the top of the document.
        public decimal AbsoluteTop
        {
            get
            {
                return Top + ((_parentElement == null) ? 0 : _parentElement.AbsoluteTop);
            }
        }

        public virtual decimal Left
        {
            set 
            {
                _left = value;
                _renderedLeft = value;
                //if (_parentElement != null)
                //    if (_parentElement.Width < _width + _left)
                //    {
                //        if (_parentElement._canGrowHorizontally)
                //            _parentElement.Width = _width + _left;
                //    }
            }
            get 
            {
                return _renderedLeft;
                //return Math.Max(_left, _renderedLeft); 
            }
        }

        // Return the top position relative to the top of the document.
        public decimal AbsoluteLeft
        {
            get
            {
                return Left + ((_parentElement == null) ? 0 : _parentElement.AbsoluteLeft);
            }
        }

        public virtual decimal Width
        {
            set 
            {
                _width = value;
                _renderedWidth = value;
                //if (_parentElement != null)
                //    if (_parentElement.Width < _width + _left)
                //    {
                //        if (_parentElement._canGrowHorizontally)
                //            _parentElement.Width = _width + _left;
                //    }
            }
            get 
            {
                if (MatchParentWidth && _renderedWidth == 0)
                    return Parent.Width;
                else
                    return _renderedWidth;
                //return Math.Max(_renderedWidth, _width);
                //if (_width == 0 && _renderedHeight == 0 && _parentElement != null)
                //    return _parentElement.Width;
                //else
                //    return Math.Max(_renderedWidth, _width); 
            }
        }

        public virtual decimal TotalWidth( bool ignoreVisibility )
        {
            return Width;
        }

        public virtual decimal Height
        {
            set 
            {
                _height = value;
                _renderedHeight = value;
                //if (_parentElement != null)
                //    if (_parentElement.Height < _height + _top)
                //    {
                //        if (_parentElement._canGrowVertically)
                //            _parentElement.Height = _height + _top;
                //        _height = _parentElement.Height - _top;
                //    }
            }
            get 
            {
                if (MatchParentHeight && _renderedHeight == 0)
                    return Parent.Height;
                else
                    return _renderedHeight;
                //return Math.Max(_renderedHeight, _height);
                //if (_height == 0 && _renderedHeight == 0 && _parentElement != null)
                //    return _parentElement.Height;
                //else
                //    return Math.Max(_renderedHeight, _height); 
            }
        }

        public virtual decimal TotalHeight( bool ignoreVisibility )
        {
            return Height;
        }

        internal bool CanGrowVertically
        {
            get { return _canGrowVertically; }
            set { _canGrowVertically = value; }
        }

        internal bool CanGrowHorizonally
        {
            get { return _canGrowHorizontally; }
            set { _canGrowHorizontally = value; }
        }

        internal Container Parent
        {
            get { return _parentElement; }
            set 
            { 
                _parentElement = value;
                //Top = _top;
                //Left = _left;
                //Width = _width;
                //Height = _height;
            }
        }

        public bool KeepTogether
        {
            set { _keepTogether = value; }
            get { return _keepTogether; }
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

        public List<Element> RepeatList
        {
            get { return _repeatList; }
        }

        internal Rdl.Engine.ReportElement ReportElement
        {
            get { return _reportElement; }
        }

        public Toggles Toggles
        {
            get { return _toggles; }
        }

        public bool IsVisible
        {
            get
            {
                if (!_toggles.IsVisible)
                    return false;
                if (_parentElement != null)
                    return _parentElement.IsVisible;
                return true;
            }
        }

        public Element FindNamedElement(string name)
        {
            Element elmt = this;
            while (elmt._parentElement != null)
                elmt = elmt._parentElement;

            return elmt.RecurseFindNamedElement(name);
        }

        private Element RecurseFindNamedElement(string name)
        {
            if (_name == name)
                return this;
            if (this is Container)
                foreach (Element child in ((Container)this).Children)
                {
                    Element e = child.RecurseFindNamedElement(name);
                    if (e != null)
                        return e;
                }
            return null;
        }

        public Rdl.Engine.Report Report
        {
            get
            {
                if (_reportElement is Rdl.Engine.Report)
                    return (Rdl.Engine.Report)_reportElement;
                if (_parentElement != null)
                    return _parentElement.Report;
                return null;
            }
        }

        public BoxStyle Style
        {
            get 
            {
                if (StyleIndex >= 0)
                    return Render.StyleList[StyleIndex];
                else
                    return null;
            }
        }

        public string Name
        {
            get { return _name; }
            set 
            { 
                _name = value + "-" + (_nameIndex++).ToString(); 
            }
        }

        public bool ContextBase
        {
            get { return _contextBase; }
            set { _contextBase = value; }
        }

        public override string ToString()
        {
            return "";
            //return ToString(0);
        }

        public string ToString(int level)
        {
            string ret = string.Empty.PadRight(level << 1) + this.GetType().FullName;
            if (this is FlowContainer)
                if (((FlowContainer)this).FlowDirection == FlowContainer.FlowDirectionEnum.LeftToRight)
                    ret += " LRT";
                else
                    ret += " TTB";
            ret += "  " + _name;
            if (this.ReportElement is Engine.ReportItem)
                ret += " " + ((Engine.ReportItem)this.ReportElement).Name;
            if (this is TextElement)
                ret += "  " + ((TextElement)this).Text;
            if (this is Table.Cell)
            {
                Table.Cell c = this as Table.Cell;
                ret += string.Format("  ({0},{1},({2},{3}))", c.Row, c.Column, c.RowSpan, c.ColSpan);
            }
            ret += "  (" + Left.ToString() + ", " + Top.ToString() + ", " + Width.ToString() + ", " + Height.ToString() + ")";
            if (Style != null && Style.BorderWidth != null)
                ret += " border( (" + Style.BorderWidth.Left.points.ToString() + ", " + Style.BorderWidth.Top.points.ToString() + ", " + Style.BorderWidth.Right.points.ToString() + ", " + Style.BorderWidth.Bottom.points.ToString() +
                    ") (" + Style.BorderStyle.Left.ToString() + ", " + Style.BorderStyle.Top.ToString() + ", " + Style.BorderStyle.Right.ToString() + ", " + Style.BorderStyle.Bottom.ToString() + ") )";

            ret += Toggles.ToString();
            ret += "\r\n";
            if (this is Container)
            {
                Container c = this as Container;
                foreach (Element e in c.Children)
                    ret += e.ToString(level+1);
            }
            return ret;
        }
    }
}
