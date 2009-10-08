using System;
using System.Collections.Generic;
using System.Text;

namespace RdlRender
{
    public abstract class Element
    {
        protected static int _nameIndex = 0;
        protected string _name = "ReportElement_" + (_nameIndex++).ToString();
        protected decimal _top = 0;
        protected decimal _left = 0;
        protected decimal _width = 0;
        protected decimal _height = 0;
        protected bool _matchParentHeight = false;
        protected bool _canGrowVertically = true;
        protected bool _canGrowHorizontally = true;
        protected Container _parentElement = null;
        internal RdlEngine.ReportElement _reportElement;
        protected bool _contextBase = false;
        // The baseStyleList is maintained only in the base element.
        // Each element uses an index into this list to locate its style.
        private List<BoxStyle> _baseStyleList;
        public int _styleIndex = -1;

        internal Element(Container parent, RdlEngine.ReportElement reportElement, BoxStyle style)
        {
            _parentElement = parent;
            _reportElement = reportElement;
            if (style != null)
                _styleIndex = AddStyle(style);
        }

        public Element(Element e)
        {
            _name = e._name;
            _canGrowHorizontally = e._canGrowHorizontally;
            _canGrowVertically = e._canGrowVertically;
            _matchParentHeight = e._matchParentHeight;
            _styleIndex = e._styleIndex;
            _reportElement = e._reportElement;
            _baseStyleList = e._baseStyleList;
            _top = e._top;
            _left = e._left;
            _width = e._width;
            _height = e._height;
        }

        public Container BaseElement()
        {
            if (_parentElement == null)
                return (Container)this;
            else
                return _parentElement.BaseElement();
        }

        protected int AddStyle(BoxStyle style)
        {
            if (_parentElement != null)
                return _parentElement.AddStyle(style);
            else
            {
                if (_baseStyleList == null)
                    _baseStyleList = new List<BoxStyle>();
                for (int i = 0; i < _baseStyleList.Count; i++)
                    if (_baseStyleList[i].GetType() == style.GetType() && _baseStyleList[i].Equals(style))
                        return i;

                _baseStyleList.Add(style);
                return _baseStyleList.Count - 1;
            }
        }


        public int StyleIndex
        {
            get { return _styleIndex; }
            set { _styleIndex = value; }
        }

        public virtual decimal Top
        {
            set 
            {
                if (_matchParentHeight)
                    _top = 0;
                else
                {
                    _top = value;
                    if (_parentElement != null)
                        if (_parentElement.Height < _height + _top)
                        {
                            if (_parentElement._canGrowVertically)
                                _parentElement.Height = _height + _top;
                        }
                }
            }
            get { return _top; }
        }

        // Return the top position relative to the top of the document.
        public decimal AbsoluteTop
        {
            get
            {
                return _top + ((_parentElement == null) ? 0 : _parentElement.AbsoluteTop);
            }
        }

        public decimal Left
        {
            set 
            { 
                _left = value;
                if (_parentElement != null)
                    if (_parentElement.Width < _width + _left)
                    {
                        if (_parentElement._canGrowHorizontally)
                            _parentElement.Width = _width + _left;
                    }
            }
            get { return _left; }
        }

        // Return the top position relative to the top of the document.
        public decimal AbsoluteLeft
        {
            get
            {
                return _left + ((_parentElement == null) ? 0 : _parentElement.AbsoluteLeft);
            }
        }

        public decimal Width
        {
            set 
            { 
                _width = value;
                if (_parentElement != null)
                    if (_parentElement.Width < _width + _left)
                    {
                        if (_parentElement._canGrowHorizontally)
                            _parentElement.Width = _width + _left;
                    }
            }
            get { return _width; }
        }

        public virtual decimal Height
        {
            set 
            {
                if (_matchParentHeight && _parentElement != null)
                    _parentElement.Height = value;
                else
                {
                    _height = value;
                    if (_parentElement != null)
                        if (_parentElement.Height < _height + _top)
                        {
                            if (_parentElement._canGrowVertically)
                                _parentElement.Height = _height + _top;
                        }
                }
            }
            get 
            {
                if (_matchParentHeight && _parentElement != null)
                    return _parentElement.Height;
                else
                    return _height; 
            }
        }

        internal bool CanGrowVertically
        {
            get { return _canGrowVertically; }
            set { _canGrowVertically = value; }
        }

        internal bool MatchParentHeight
        {
            get { return _matchParentHeight; }
            set { _matchParentHeight = value; }
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
                Top = _top;
                Left = _left;
                Width = _width;
                Height = _height;
            }
        }

        internal RdlEngine.ReportElement ReportElement
        {
            get { return _reportElement; }
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

        public RdlEngine.Report Report
        {
            get
            {
                if (_reportElement is RdlEngine.Report)
                    return (RdlEngine.Report)_reportElement;
                if (_parentElement != null)
                    return _parentElement.Report;
                return null;
            }
        }

        public List<BoxStyle> BaseStyleList
        {
            get
            {
                if (_baseStyleList != null)
                    return _baseStyleList;
                else if (_parentElement != null)
                    return _parentElement.BaseStyleList;
                return null;
            }
            set
            {
                _baseStyleList = value;
            }
        }

        public BoxStyle Style
        {
            get 
            {
                if (StyleIndex >= 0 && BaseStyleList != null)
                    return BaseStyleList[StyleIndex];
                else
                    return null;
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value + "-" + (_nameIndex++).ToString(); }
        }

        public bool ContextBase
        {
            get { return _contextBase; }
            set { _contextBase = value; }
        }

        public override string ToString()
        {
            return ToString(0);
        }

        private string ToString(int level)
        {
            string ret = string.Empty.PadRight(level << 1) + this.GetType().Name + "  " + _name;
            if (this is TextElement)
                ret += "  " + ((TextElement)this).Text;
            ret += "  (" + _left.ToString() + ", " + _top.ToString() + ", " + _width.ToString() + ", " + _height.ToString() + ")";
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
