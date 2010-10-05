using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rdl.Engine
{
    public class Style : ReportElement
    {
        public enum GradientTypeEnum
        {
            None,
            LeftRight,
            TopBottom,
            Center,
            DiagonalLeft,
            DiagonalRight,
            HorizontalCenter,
            VerticalCenter
        };

        public enum FontStyleEnum
        {
            Normal,
            Italic
        };

        public enum FontWeightEnum
        {
            Lighter,
            _100,
            Normal,
            _200,
            Bold,
            Bolder,
            _300,
            _400,
            _500,
            _600,
            _700,
            _800,
            _900
        }

        public enum TextDecorationEnum
        {
            Underline,
            Overline,
            LineThrough,
            None
        }

        public enum TextAlignEnum
        {
            Left,
            Center,
            Right,
            General
        }

        public enum VerticalAlignEnum
        {
            Top,
            Middle,
            Bottom
        }

        public enum DirectionEnum
        {
            LTR,
            RTL
        }

        public enum WritingModeEnum
        {
            lr_tb,
            tb_rl
        }

        public enum UnicodeBiDiEnum
        {
            Normal,
            Embed,
            BiDi_override
        }

        private BorderColor _borderColor = null;
        private BorderStyle _borderStyle = null;
        private BorderWidth _borderWidth = null;
        private Expression _backgroundColor = null;
        private Expression _backgroundGradientType = null;
        private Expression _backgroundGradientEndColor = null;
        private BackgroundImage _backgroundImage = null;
        private Expression _fontStyle = null;
        private Expression _fontFamily = null;
        private Expression _fontSize = null;
        private Expression _fontWeight = null;
        private Expression _format = null;
        private Expression _textDecoration = null;
        private Expression _textAlign = null;
        private Expression _verticalAlign = null;
        private Expression _color = null;
        private Expression _paddingLeft = null;
        private Expression _paddingRight = null;
        private Expression _paddingTop = null;
        private Expression _paddingBottom = null;
        private Expression _lineHeight = null;
        private Expression _direction = null;
        private Expression _writingMode = null;
        private Expression _language = null;
        private Expression _unicodeBiDi = null;
        private Expression _calendar = null;
        private Expression _numeralLanguage = null;
        private Expression _numeralVariant = null;
        public static Style DefaultStyle = new Style(null, null);

        public Style(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "bordercolor":
                    _borderColor = new BorderColor(attr, this);
                    break;
                case "borderstyle":
                    _borderStyle = new BorderStyle(attr, this);
                    break;
                case "borderwidth":
                    _borderWidth = new BorderWidth(attr, this);
                    break;
                case "backgroundcolor":
                    _backgroundColor = new Expression(attr, this);
                    break;
                case "backgroundgradienttype":
                    _backgroundGradientType = new Expression(attr, this);
                    break;
                case "backgroundgradientendcolor":
                    _backgroundGradientEndColor = new Expression(attr, this);
                    break;
                case "backgroundimage":
                    _backgroundImage = new BackgroundImage(attr, this);
                    break;
                case "fontfamily":
                    _fontFamily = new Expression(attr, this);
                    break;
                case "fontsize":
                    _fontSize = new Expression(attr, this);
                    break;
                case "fontweight":
                    _fontWeight = new Expression(attr, this);
                    break;
                case "format":
                    _format = new Expression(attr, this);
                    break;
                case "textdecoration":
                    _textDecoration = new Expression(attr, this);
                    break;
                case "textalign":
                    _textAlign = new Expression(attr, this);
                    break;
                case "verticalalign":
                    _verticalAlign = new Expression(attr, this);
                    break;
                case "color":
                    _color = new Expression(attr, this);
                    break;
                case "paddingleft":
                    _paddingLeft = new Expression(attr, this);
                    break;
                case "paddingtop":
                    _paddingTop = new Expression(attr, this);
                    break;
                case "paddingbottom":
                    _paddingBottom = new Expression(attr, this);
                    break;
                case "paddingright":
                    _paddingRight = new Expression(attr, this);
                    break;
                case "lineheight":
                    _lineHeight = new Expression(attr, this);
                    break;
                case "direction":
                    _direction = new Expression(attr, this);
                    break;
                case "writingmode":
                    _writingMode = new Expression(attr, this);
                    break;
                case "language":
                    _language = new Expression(attr, this);
                    break;
                case "unicodebidi":
                    _unicodeBiDi = new Expression(attr, this);
                    break;
                case "calendar":
                    _calendar = new Expression(attr, this);
                    break;
                case "numerallanguage":
                    _numeralLanguage = new Expression(attr, this);
                    break;
                case "numeralvariant":
                    _numeralVariant = new Expression(attr, this);
                    break;
                default:
                    break;
            }
        }

        public static System.Drawing.Color W32Color(string name)
        {
            switch (name)
            {
                case "LightGrey":
                    return System.Drawing.Color.FromName("LightGray");
                default:
                    return System.Drawing.Color.FromName(name);
            }
        }

        public BorderColor BorderColor
        {
            get
            {
                if (_borderColor == null)
                {
                    if (Parent != null && Parent.Parent != null)
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.BorderColor;
                    }
                    _borderColor = new BorderColor(null, this);
                }
                return _borderColor;
            }
        }

        public BorderStyle BorderStyle
        {
            get
            {
                if (_borderStyle == null)
                {
                    if (Parent != null && Parent.Parent != null )
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.BorderStyle;
                    }
                    _borderStyle = new BorderStyle(null, this);
                }
                return _borderStyle;
            }
        }

        public BorderWidth BorderWidth
        {
            get
            {
                if (_borderWidth == null)
                {
                    if (Parent != null && Parent.Parent != null )
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.BorderWidth;
                    }
                    _borderWidth = new BorderWidth(null, this);
                }
                return _borderWidth;
            }
        }

        public string BackgroundColor(Rdl.Runtime.Context context)
        {
            string color = null;
            if (_backgroundColor != null)
                color = _backgroundColor.ExecAsString(context);
            if (color == null)
            {
                if (Parent != null && Parent.Parent != null)
                {
                    Style parentStyle = Parent.Parent.Style;
                    if (parentStyle != null)
                        color = parentStyle.BackgroundColor(context);
                }
                else
                    color = "transparent";
            }
            return color;
        }

        public GradientTypeEnum BackgroundGradientType(Rdl.Runtime.Context context)
        {
            if (_backgroundGradientType == null)
            {
                if (Parent != null && Parent.Parent != null )
                {
                    Style parentStyle = Parent.Parent.Style;
                    if (parentStyle != null)
                        return parentStyle.BackgroundGradientType(context);
                }
                _backgroundGradientType = new Expression("None", this);
            }
            return (GradientTypeEnum)Enum.Parse(typeof(GradientTypeEnum), _backgroundGradientType.ExecAsString(context), true);
        }

        public string BackgroundGradientEndColor(Rdl.Runtime.Context context)
        {
            if (_backgroundGradientEndColor == null)
            {
                if (Parent != null && Parent.Parent != null )
                {
                    Style parentStyle = Parent.Parent.Style;
                    if (parentStyle != null)
                        return parentStyle.BackgroundGradientEndColor(context);
                }
                return Color(context);
            }
            return _backgroundGradientEndColor.ExecAsString(context);
        }

        public BackgroundImage BackgroundImage
        {
            get
            {
                return _backgroundImage;
            }
        }

        public FontStyleEnum FontStyle(Rdl.Runtime.Context context)
        {
            if (_fontStyle == null)
            {
                if (Parent != null && Parent.Parent != null )
                {
                    Style parentStyle = Parent.Parent.Style;
                    if (parentStyle != null)
                        return parentStyle.FontStyle(context);
                }
                _fontStyle = new Expression("Normal", this);
            }
            return (FontStyleEnum)Enum.Parse(typeof(FontStyleEnum), _fontStyle.ExecAsString(context), true);
        }

        public string FontFamily(Rdl.Runtime.Context context)
        {
            if (_fontFamily == null)
            {
                if (Parent != null && Parent.Parent != null )
                {
                    Style parentStyle = Parent.Parent.Style;
                    if (parentStyle != null)
                        return parentStyle.FontFamily(context);
                }
                _fontFamily = new Expression("Arial", this);
            }
            return _fontFamily.ExecAsString(context);
        }

        public Size FontSize(Rdl.Runtime.Context context)
        {
            if (_fontSize == null)
            {
                if (Parent != null && Parent.Parent != null )
                {
                    Style parentStyle = Parent.Parent.Style;
                    if (parentStyle != null)
                        return parentStyle.FontSize(context);
                }
                _fontSize = new Expression("10pt", this);
            }
            Size s = new Size(_fontSize.ExecAsString(context));
            if (s.points <= 0)
                s = new Size("1pt");
            return s;
        }

        public FontWeightEnum FontWeight(Rdl.Runtime.Context context)
        {
            if (_fontWeight == null)
            {
                if (Parent != null && Parent.Parent != null )
                {
                    Style parentStyle = Parent.Parent.Style;
                    if (parentStyle != null)
                        return parentStyle.FontWeight(context);
                }
                _fontWeight = new Expression("Normal", this);
            }
            string str = _fontWeight.ExecAsString(context);
            if (str[0] >= '1' && str[0] <= '9')
                str = "_" + str;

            return (FontWeightEnum)Enum.Parse(typeof(FontWeightEnum), str, true);
        }

        public string Format(Rdl.Runtime.Context context)
        {
            if (_format == null)
            {
                if (Parent != null && Parent.Parent != null )
                {
                    Style parentStyle = Parent.Parent.Style;
                    if (parentStyle != null)
                        return parentStyle.Format(context);
                }
                _format = new Expression("", this);
            }
            return _format.ExecAsString(context);
        }

        public TextDecorationEnum TextDecoration(Rdl.Runtime.Context context)
        {
            if (_textDecoration == null)
            {
                if (Parent != null && Parent.Parent != null )
                {
                    Style parentStyle = Parent.Parent.Style;
                    if (parentStyle != null)
                        return parentStyle.TextDecoration(context);
                }
                _textDecoration = new Expression("None", this);
            }
            return (TextDecorationEnum)Enum.Parse(typeof(TextDecorationEnum), _textDecoration.ExecAsString(context), true);
        }

        public TextAlignEnum TextAlign(Rdl.Runtime.Context context)
        {
            if (_textAlign == null)
            {
                if (Parent != null && Parent.Parent != null )
                {
                    Style parentStyle = Parent.Parent.Style;
                    if (parentStyle != null)
                        return parentStyle.TextAlign(context);
                }
                _textAlign = new Expression("General", this);
            }
            return (TextAlignEnum)Enum.Parse(typeof(TextAlignEnum), _textAlign.ExecAsString(context), true);
        }

        public VerticalAlignEnum VerticalAlign(Rdl.Runtime.Context context)
        {
            if (_verticalAlign == null)
            {
                if (Parent != null && Parent.Parent != null )
                {
                    Style parentStyle = Parent.Parent.Style;
                    if (parentStyle != null)
                        return parentStyle.VerticalAlign(context);
                }
                _verticalAlign = new Expression("Top", this);
            }
            return (VerticalAlignEnum)Enum.Parse(typeof(VerticalAlignEnum), _verticalAlign.ExecAsString(context), true);
        }

        public string Color(Rdl.Runtime.Context context)
        {
            if (_color == null)
            {
                if (Parent != null && Parent.Parent != null )
                {
                    Style parentStyle = Parent.Parent.Style;
                    if (parentStyle != null)
                        return parentStyle.Color(context);
                }
                _color = new Expression("Black", this);
            }
            return _color.ExecAsString(context);
        }

        public Size PaddingLeft(Rdl.Runtime.Context context)
        {
            if (_paddingLeft == null)
            {
                if (Parent != null && Parent.Parent != null )
                {
                    Style parentStyle = Parent.Parent.Style;
                    if (parentStyle != null)
                        return parentStyle.PaddingLeft(context);
                }
                _paddingLeft = new Expression("0pt", this);
            }
            return new Size(_paddingLeft.ExecAsString(context));
        }

        public Size PaddingRight(Rdl.Runtime.Context context)
        {
            if (_paddingRight == null)
            {
                if (Parent != null && Parent.Parent != null )
                {
                    Style parentStyle = Parent.Parent.Style;
                    if (parentStyle != null)
                        return parentStyle.PaddingRight(context);
                }
                _paddingRight = new Expression("0pt", this);
            }
            return new Size(_paddingRight.ExecAsString(context));
        }

        public Size PaddingTop(Rdl.Runtime.Context context)
        {
            if (_paddingTop == null)
            {
                if (Parent != null && Parent.Parent != null )
                {
                    Style parentStyle = Parent.Parent.Style;
                    if (parentStyle != null)
                        return parentStyle.PaddingTop(context);
                }
                _paddingTop = new Expression("0pt", this);
            }
            return new Size(_paddingTop.ExecAsString(context));
        }

        public Size PaddingBottom(Rdl.Runtime.Context context)
        {
            if (_paddingBottom == null)
            {
                if (Parent != null && Parent.Parent != null )
                {
                    Style parentStyle = Parent.Parent.Style;
                    if (parentStyle != null)
                        return parentStyle.PaddingBottom(context);
                }
                _paddingBottom = new Expression("0pt", this);
            }
            return new Size(_paddingBottom.ExecAsString(context));
        }

        public Size LineHeight(Rdl.Runtime.Context context)
        {
            if (_lineHeight == null)
            {
                _lineHeight = new Expression((FontSize(context).points + 2m).ToString() + "pt", this);
            }
            return new Size(_lineHeight.ExecAsString(context));
        }

        public DirectionEnum Direction(Rdl.Runtime.Context context)
        {
            if (_direction == null)
            {
                if (Parent != null && Parent.Parent != null )
                {
                    Style parentStyle = Parent.Parent.Style;
                    if (parentStyle != null)
                        return parentStyle.Direction(context);
                }
                _direction = new Expression("LTR", this);
            }
            return (DirectionEnum)Enum.Parse(typeof(DirectionEnum), _direction.ExecAsString(context), true);
        }

        public WritingModeEnum WritingMode(Rdl.Runtime.Context context)
        {
            if (_writingMode == null)
            {
                if (Parent != null && Parent.Parent != null )
                {
                    Style parentStyle = Parent.Parent.Style;
                    if (parentStyle != null)
                        return parentStyle.WritingMode(context);
                }
                _writingMode = new Expression("lr-tb", this);
            }
            return (WritingModeEnum)Enum.Parse(typeof(WritingModeEnum), _writingMode.ExecAsString(context).Replace('-', '_'), true);
        }

        public string Language(Rdl.Runtime.Context context)
        {
            if (_language == null)
            {
                if (Parent != null && Parent.Parent != null )
                {
                    Style parentStyle = Parent.Parent.Style;
                    if (parentStyle != null)
                        return parentStyle.Language(context);
                }
                if (this.Report != null)
                    return this.Report.Language(context);
                else
                    return string.Empty;
            }
            return _language.ExecAsString(context);
        }

        public UnicodeBiDiEnum UnicodeBiDi(Rdl.Runtime.Context context)
        {
            if (_unicodeBiDi == null)
            {
                if (Parent != null && Parent.Parent != null )
                {
                    Style parentStyle = Parent.Parent.Style;
                    if (parentStyle != null)
                        return parentStyle.UnicodeBiDi(context);
                }
                _unicodeBiDi = new Expression("Normal", this);
            }
            return (UnicodeBiDiEnum)Enum.Parse(typeof(UnicodeBiDiEnum), _unicodeBiDi.ExecAsString(context), true);
        }

        public string Calendar(Rdl.Runtime.Context context)
        {
            if (_calendar == null)
            {
                if (Parent != null && Parent.Parent != null )
                {
                    Style parentStyle = Parent.Parent.Style;
                    if (parentStyle != null)
                        return parentStyle.Calendar(context);
                }
                // TBD - I don't understand this property yet.
                _calendar = new Expression("", this);
            }
            return _calendar.ExecAsString(context);
        }

        public string NumeralLanguage(Rdl.Runtime.Context context)
        {
            if (_numeralLanguage == null)
            {
                if (Parent != null && Parent.Parent != null )
                {
                    Style parentStyle = Parent.Parent.Style;
                    if (parentStyle != null)
                        return parentStyle.NumeralLanguage(context);
                }
                return Language(context);
            }
            return _numeralLanguage.ExecAsString(context);
        }

        public int NumeralVariant(Rdl.Runtime.Context context)
        {
            if (_numeralVariant == null)
            {
                if (Parent != null && Parent.Parent != null )
                {
                    Style parentStyle = Parent.Parent.Style;
                    if (parentStyle != null)
                        return parentStyle.NumeralVariant(context);
                }
                _numeralVariant = new Expression("1", this);
            }
            return int.Parse(_numeralVariant.ExecAsString(context));
        }

        public System.Drawing.Font GetWindowsFont(Rdl.Runtime.Context context)
        {
            System.Drawing.FontStyle fs = (FontStyle(context) == FontStyleEnum.Italic) ? System.Drawing.FontStyle.Italic : System.Drawing.FontStyle.Regular;
            if (FontWeight(context) >= FontWeightEnum.Bold)
                fs |= System.Drawing.FontStyle.Bold;
            if (TextDecoration(context) == TextDecorationEnum.Underline)
                fs |= System.Drawing.FontStyle.Underline;
            if (TextDecoration(context) == TextDecorationEnum.LineThrough)
                fs |= System.Drawing.FontStyle.Strikeout;
            return new System.Drawing.Font(FontFamily(context), (float)FontSize(context).points, fs, System.Drawing.GraphicsUnit.Point);
        }
    }
}
