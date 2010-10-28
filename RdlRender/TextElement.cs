using System;
using System.Collections.Generic;
using System.Text;

namespace RdlRender
{
    public class TextElement : Element
    {
        public enum ToggleStateEnum
        {
            open,
            closed
        }

        private string _text;
        private ToggleStateEnum _toggleState = ToggleStateEnum.closed;
        private List<Container> _toggleList = new List<Container>();
        // _toggle is used to link a TextElement in a page render to
        // the TextElement in the master rendered document.
        private TextElement _sourceElement = null;

        internal TextElement(Container parent, RdlEngine.ReportElement reportElement, string name, string text, BoxStyle style)
            : base(parent, reportElement, style)
        {
            _name = name;
            _text = text;
            if (reportElement is RdlEngine.TextBox)
                if (((RdlEngine.TextBox)reportElement).ToggleImage != null)
                    _toggleState = (((RdlEngine.TextBox)reportElement).ToggleImage.InitialState) ? ToggleStateEnum.open : ToggleStateEnum.closed;
        }

        public TextElement(TextElement t)
            : base(t)
        {
            _text = t._text;
            _toggleState = t._toggleState;
            _toggleList = t._toggleList;
            _sourceElement = t;
        }

        public string Text
        {
            get { return _text; }
        }

        public ToggleStateEnum ToggleState
        {
            get 
            {
                if (_sourceElement == null)
                    return _toggleState;
                else
                    return _sourceElement._toggleState;
            }
            set 
            {
                if (_sourceElement == null)
                    _toggleState = value;
                else
                    _sourceElement._toggleState = value;
            }
        }

        internal void LinkToggles()
        {
            if (((RdlEngine.TextBox)ReportElement).ToggleList.Count > 0)
            {
                // Find the context base.  This is where the nearest parent context
                // is defined and is the starting point to start searching
                // the tree for the referenced element.
                Container parent = Parent;
                while (parent != null && !parent.ContextBase)
                    parent = parent.Parent;

                foreach (RdlEngine.ReportElement r in ((RdlEngine.TextBox)ReportElement).ToggleList)
                {
                    List<Container> bl = parent.FindReportElements(r);
                    foreach (Container b in bl)
                    {
                        _toggleList.Add(b);
                        b.Toggle = this;
                    }
                }
            }
        }

        public List<Container> ToggleList
        {
            get { return _toggleList; }
        }
    }
}
