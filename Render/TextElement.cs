using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Render
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
        // the TextElement in the master rendered document.
        private TextElement _sourceElement = null;
        private bool _isToggle = false;
        public List<Element> LinkedToggles = new List<Element>();

        internal TextElement(Container parent, Rdl.Engine.ReportElement reportElement, string name, string text, BoxStyle style, Rdl.Runtime.Context context)
            : base(parent, reportElement, style)
        {
            _name = name;
            _text = text;
            Rdl.Engine.TextBox tb = reportElement as Rdl.Engine.TextBox;
            if (tb != null)
            {
                if (((Rdl.Engine.TextBox)reportElement).ToggleImage != null)
                    _toggleState = (((Rdl.Engine.TextBox)reportElement).ToggleImage.InitialState(context)) ? ToggleStateEnum.open : ToggleStateEnum.closed;
            }
        }

        public TextElement(TextElement t)
            : base(t)
        {
            _text = t._text;
            _toggleState = t._toggleState;
            _isToggle = t._isToggle;
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

        public bool IsToggle
        {
            set { _isToggle = value; }
            get { return _isToggle; }
        }
    }
}
