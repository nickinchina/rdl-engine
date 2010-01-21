using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RdlEngine
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
            Normal,
            Bold,
            Bolder,
            _100,
            _200,
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

        public string BackgroundColor
        {
            get
            {
                if (_backgroundColor == null)
                {
                    if (Parent != null && Parent.Parent != null )
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.BackgroundColor;
                    }
                    _backgroundColor = new Expression("transparent", this);
                }
                return _backgroundColor.ExecAsString();
            }
        }

        public GradientTypeEnum BackgroundGradientType
        {
            get
            {
                if (_backgroundGradientType == null)
                {
                    if (Parent != null && Parent.Parent != null )
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.BackgroundGradientType;
                    }
                    _backgroundGradientType = new Expression("None", this);
                }
                return (GradientTypeEnum)Enum.Parse(typeof(GradientTypeEnum), _backgroundGradientType.ExecAsString(), true);
            }
        }

        public string BackgroundGradientEndColor
        {
            get
            {
                if (_backgroundGradientEndColor == null)
                {
                    if (Parent != null && Parent.Parent != null )
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.BackgroundGradientEndColor;
                    }
                    return Color;
                }
                return _backgroundGradientEndColor.ExecAsString();
            }
        }

        public BackgroundImage BackgroundImage
        {
            get
            {
                if (_backgroundImage == null)
                {
                    if (Parent != null && Parent.Parent != null )
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.BackgroundImage;
                    }
                    _backgroundImage = new BackgroundImage(null, this);
                }
                return _backgroundImage;
            }
        }

        public FontStyleEnum FontStyle
        {
            get
            {
                if (_fontStyle == null)
                {
                    if (Parent != null && Parent.Parent != null )
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.FontStyle;
                    }
                    _fontStyle = new Expression("Normal", this);
                }
                return (FontStyleEnum)Enum.Parse(typeof(FontStyleEnum), _fontStyle.ExecAsString(), true);
            }
        }

        public string FontFamily
        {
            get
            {
                if (_fontFamily == null)
                {
                    if (Parent != null && Parent.Parent != null )
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.FontFamily;
                    }
                    _fontFamily = new Expression("Arial", this);
                }
                return _fontFamily.ExecAsString();
            }
        }

        public Size FontSize
        {
            get
            {
                if (_fontSize == null)
                {
                    if (Parent != null && Parent.Parent != null )
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.FontSize;
                    }
                    _fontSize = new Expression("10pt", this);
                }
                return new Size(_fontSize.ExecAsString());
            }
        }

        public FontWeightEnum FontWeight
        {
            get
            {
                if (_fontWeight == null)
                {
                    if (Parent != null && Parent.Parent != null )
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.FontWeight;
                    }
                    _fontWeight = new Expression("Normal", this);
                }
                string str = _fontWeight.ExecAsString();
                if (str[0] >= '1' && str[0] <= '9')
                    str = "_" + str;

                return (FontWeightEnum)Enum.Parse(typeof(FontWeightEnum), str, true);
            }
        }

        public string Format
        {
            get
            {
                if (_format == null)
                {
                    if (Parent != null && Parent.Parent != null )
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.Format;
                    }
                    _format = new Expression("", this);
                }
                return _format.ExecAsString();
            }
        }

        public TextDecorationEnum TextDecoration
        {
            get
            {
                if (_textDecoration == null)
                {
                    if (Parent != null && Parent.Parent != null )
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.TextDecoration;
                    }
                    _textDecoration = new Expression("None", this);
                }
                return (TextDecorationEnum)Enum.Parse(typeof(TextDecorationEnum), _textDecoration.ExecAsString(), true);
            }
        }

        public TextAlignEnum TextAlign
        {
            get
            {
                if (_textAlign == null)
                {
                    if (Parent != null && Parent.Parent != null )
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.TextAlign;
                    }
                    _textAlign = new Expression("General", this);
                }
                return (TextAlignEnum)Enum.Parse(typeof(TextAlignEnum), _textAlign.ExecAsString(), true);
            }
        }

        public VerticalAlignEnum VerticalAlign
        {
            get
            {
                if (_verticalAlign == null)
                {
                    if (Parent != null && Parent.Parent != null )
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.VerticalAlign;
                    }
                    _verticalAlign = new Expression("Top", this);
                }
                return (VerticalAlignEnum)Enum.Parse(typeof(VerticalAlignEnum), _verticalAlign.ExecAsString(), true);
            }
        }

        public string Color
        {
            get
            {
                if (_color == null)
                {
                    if (Parent != null && Parent.Parent != null )
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.Color;
                    }
                    _color = new Expression("Black", this);
                }
                return _color.ExecAsString();
            }
        }

        public Size PaddingLeft
        {
            get
            {
                if (_paddingLeft == null)
                {
                    if (Parent != null && Parent.Parent != null )
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.PaddingLeft;
                    }
                    _paddingLeft = new Expression("0pt", this);
                }
                return new Size(_paddingLeft.ExecAsString());
            }
        }

        public Size PaddingRight
        {
            get
            {
                if (_paddingRight == null)
                {
                    if (Parent != null && Parent.Parent != null )
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.PaddingRight;
                    }
                    _paddingRight = new Expression("0pt", this);
                }
                return new Size(_paddingRight.ExecAsString());
            }
        }

        public Size PaddingTop
        {
            get
            {
                if (_paddingTop == null)
                {
                    if (Parent != null && Parent.Parent != null )
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.PaddingTop;
                    }
                    _paddingTop = new Expression("0pt", this);
                }
                return new Size(_paddingTop.ExecAsString());
            }
        }

        public Size PaddingBottom
        {
            get
            {
                if (_paddingBottom == null)
                {
                    if (Parent != null && Parent.Parent != null )
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.PaddingBottom;
                    }
                    _paddingBottom = new Expression("0pt", this);
                }
                return new Size(_paddingBottom.ExecAsString());
            }
        }

        public Size LineHeight
        {
            get
            {
                if (_lineHeight == null)
                {
                    _lineHeight = new Expression((FontSize.points + 2m).ToString() + "pt", this);
                }
                return new Size(_lineHeight.ExecAsString());
            }
        }

        public DirectionEnum Direction
        {
            get
            {
                if (_direction == null)
                {
                    if (Parent != null && Parent.Parent != null )
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.Direction;
                    }
                    _direction = new Expression("LTR", this);
                }
                return (DirectionEnum)Enum.Parse(typeof(DirectionEnum), _direction.ExecAsString(), true);
            }
        }

        public WritingModeEnum WritingMode
        {
            get
            {
                if (_writingMode == null)
                {
                    if (Parent != null && Parent.Parent != null )
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.WritingMode;
                    }
                    _writingMode = new Expression("lr-tb", this);
                }
                return (WritingModeEnum)Enum.Parse(typeof(WritingModeEnum), _writingMode.ExecAsString().Replace('-', '_'), true);
            }
        }

        public string Language
        {
            get
            {
                if (_language == null)
                {
                    if (Parent != null && Parent.Parent != null )
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.Language;
                    }
                    if (this.Report != null)
                        return this.Report.Language;
                    else
                        return string.Empty;
                }
                return _language.ExecAsString();
            }
        }

        public UnicodeBiDiEnum UnicodeBiDi
        {
            get
            {
                if (_unicodeBiDi == null)
                {
                    if (Parent != null && Parent.Parent != null )
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.UnicodeBiDi;
                    }
                    _unicodeBiDi = new Expression("Normal", this);
                }
                return (UnicodeBiDiEnum)Enum.Parse(typeof(UnicodeBiDiEnum), _unicodeBiDi.ExecAsString(), true);
            }
        }

        public string Calendar
        {
            get
            {
                if (_calendar == null)
                {
                    if (Parent != null && Parent.Parent != null )
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.Calendar;
                    }
                    // TBD - I don't understand this property yet.
                    _calendar = new Expression("", this);
                }
                return _calendar.ExecAsString();
            }
        }

        public string NumeralLanguage
        {
            get
            {
                if (_numeralLanguage == null)
                {
                    if (Parent != null && Parent.Parent != null )
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.NumeralLanguage;
                    }
                    return Language;
                }
                return _numeralLanguage.ExecAsString();
            }
        }

        public int NumeralVariant
        {
            get
            {
                if (_numeralVariant == null)
                {
                    if (Parent != null && Parent.Parent != null )
                    {
                        Style parentStyle = Parent.Parent.Style;
                        if (parentStyle != null)
                            return parentStyle.NumeralVariant;
                    }
                    _numeralVariant = new Expression("1", this);
                }
                return int.Parse(_numeralVariant.ExecAsString());
            }
        }

    }
}
