using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Rdl.Render;

namespace Rdl.Engine
{
    class TextBox : ReportItem
    {
        private enum DataElementStyleEnum
        {
            Auto,
            AttributeNormal,
            ElementNormal
        };

        private Expression _vaue = null;
        private bool _canGrow = false;
        private bool _canShrink = false;
        private string _hideDuplicates = null;
        private ToggleImage _toggleImage = null;
        private UserSort _userSort = null;
        private Enums.DataElementOutputEnum _dataElementStyle = Enums.DataElementOutputEnum.Auto;
        private Expression _initialState = null;
        private string _lastValue = string.Empty;
        private Int32 _lastGroupIndex = -1;
        internal List<Toggle> LinkedToggles = new List<Toggle>();
        //internal Rdl.Render.TextElement TextElement;

        private string _text = string.Empty;

        public TextBox(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            base.ParseAttribute(attr);
            switch (attr.Name.ToLower())
            {
                case "value":
                    _vaue = new Expression(attr, this);
                    break;
                case "cangrow":
                    _canGrow = bool.Parse(attr.InnerText);
                    break;
                case "canshrink":
                    _canShrink = bool.Parse(attr.InnerText);
                    break;
                case "hideduplicates":
                    _hideDuplicates = attr.InnerText;
                    break;
                case "toggleimage":
                    _toggleImage = new ToggleImage(attr, this);
                    break;
                case "usersort":
                    _userSort = new UserSort(attr, this);
                    break;
                case "dataelementstyle":
                    _dataElementStyle = (Enums.DataElementOutputEnum)Enum.Parse(typeof(Enums.DataElementOutputEnum), attr.InnerText, true);
                    break;
                case "initialstate":
                    _initialState = new Expression(attr, this);
                    break;
                default:
                    break;
            }
        }

        public ToggleImage ToggleImage
        {
            get { return _toggleImage; }
        }

        public Rdl.Render.TextElement TextElement
        {
            get { return (Rdl.Render.TextElement)_box; }
        }

        protected override void Render1(Rdl.Render.Container parentBox, Rdl.Runtime.Context context, bool visible)
        {
            _text = string.Empty;
            // Link the textboxes occuring at the current context level.
            context.TextBoxes[Name] = this;

            try
            {
                object o = _vaue.Exec(context);
                if (!Convert.IsDBNull(o))
                    if (Style != null && Style.Format(context) != string.Empty)
                        _text = String.Format("{0:" + Style.Format(context) + "}", o);
                    else
                        _text = o.ToString();
            }
            catch (Exception err)
            {
                throw new Exception("Error evaluating expression in TextBox " + _name, err);
            }
            bool hidden = false;

            if (_hideDuplicates != null)
            {
                int gi = context.FindContextByGroupName(_hideDuplicates, this.Report).GroupIndex;
                if (_text == _lastValue)
                {
                    if (gi == _lastGroupIndex)
                        hidden = true;
                }
                _lastGroupIndex = gi;
            }

            if (visible && parentBox != null)
            {
                _box = parentBox.AddTextElement(this, _name, (hidden)?string.Empty:_text, Style, context);
                _box.Name = "TextBox";
                //TextElement = (Rdl.Render.TextElement)_box;
            }

            _lastValue = _text;
        }

        protected override void Render2(Rdl.Runtime.Context context)
        {
            if (_box != null)
            {
                decimal width = _box.Width;
                decimal height = _box.Height;

                if (_canGrow || _canShrink)
                {
                    // If the textbox can shrink or grow based on the contained text
                    // then we need to measure the text and set the box size appropriately.

                    System.Drawing.Font font = ((Rdl.Render.TextStyle)_box.Style).GetWindowsFont();
                    System.Drawing.Bitmap bm = new System.Drawing.Bitmap(10, 10);
                    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bm);
                    g.PageUnit = System.Drawing.GraphicsUnit.Point;
                    System.Drawing.SizeF textSize = g.MeasureString(
                        _text, font, (int)(width - _box.Style.PaddingLeft.points - _box.Style.PaddingRight.points));

                    decimal h = (decimal)textSize.Height + _box.Style.PaddingTop.points + _box.Style.PaddingBottom.points;
                    if (_canGrow && h > height)
                        height = h;
                    if (_canShrink && h < height)
                        height = h;
                }
                _box.Width = width;
                _box.Height = height;
            }
        }
    }
}
