using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using RdlRender;

namespace RdlEngine
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
        private List<ReportElement> _toggleList = new List<ReportElement>();

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

        public override void Render(RdlRender.Container box)
        {
            if (box != null)
            {
                string text;
                try
                {
                    text = _vaue.ExecAsString((Style==null)?"":Style.Format);
                }
                catch (Exception err)
                {
                    throw new Exception("Error evaluating expression in TextBox " + _name, err);
                }
                bool hidden = false;

                if (_hideDuplicates != null)
                {
                    int gi = Context.FindContextByGroupName(_hideDuplicates).GroupIndex;
                    if (text == _lastValue)
                    {
                        if (gi == _lastGroupIndex)
                            hidden = true;
                    }
                    _lastGroupIndex = gi;
                }

                if (!hidden)
                {
                    decimal width = (IsInCell || _width == null) ? box.Width : _width.points;
                    decimal height = (IsInCell) ? box.Height : _height.points;

                    TextElement te = box.AddTextElement(this, _name, text, Style);
                    te.Name = "TextBox";
                    if (_canGrow || _canShrink)
                    {
                        // If the textbox can shrink or grow based on the contained text
                        // then we need to measure the text and set the box size appropriately.
                        Style style = this.Style;

                        System.Drawing.Bitmap bm = new System.Drawing.Bitmap((int)width, (int)height);
                        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bm);
                        g.PageUnit = System.Drawing.GraphicsUnit.Point;
                        System.Drawing.FontStyle fs = (Style.FontStyle == Style.FontStyleEnum.Italic) ? System.Drawing.FontStyle.Italic : System.Drawing.FontStyle.Regular;
                        if (Style.FontWeight >= RdlEngine.Style.FontWeightEnum.Bold)
                            fs |= System.Drawing.FontStyle.Bold;
                        System.Drawing.Font font = new System.Drawing.Font(
                            style.FontFamily, (float)style.FontSize.points, fs);
                        System.Drawing.SizeF textSize = g.MeasureString(
                            text, font, (int)width);
//                        System.Drawing.Size textSize = 
//                            System.Windows.Forms.TextRenderer.MeasureText(
//                                text, font, 
//                                new System.Drawing.Size((int)width, int.MaxValue));

                        if (_canGrow && (decimal)textSize.Height > height)
                            height = (decimal)textSize.Height;
                        if (_canShrink && (decimal)textSize.Height < height)
                            height = (decimal)textSize.Height;
                    }
                    if (IsInCell)
                    {
                        te.MatchParentHeight = true;
                    }
                    else
                    {
                        te.Left = _left.points;
                        te.Top = _top.points;
                    }
                    te.Width = width;
                    te.Height = height;

                    _lastValue = text;
                }
            }
        }

        internal List<ReportElement> ToggleList
        {
            get { return _toggleList; }
        }
    }
}
