using System;
using System.Collections.Generic;
using System.Text;

namespace RdlRender
{
    public class TextStyle : BoxStyle
    {
        public RdlEngine.Style.FontStyleEnum FontStyle;
        public string FontFamily;
        public RdlEngine.Size FontSize;
        public RdlEngine.Style.FontWeightEnum FontWeight;
        public string Format;
        public RdlEngine.Style.TextDecorationEnum TextDecoration;
        public RdlEngine.Style.TextAlignEnum TextAlign;
        public RdlEngine.Style.VerticalAlignEnum VerticalAlign;
        public RdlEngine.Size LineHeight;
        public RdlEngine.Style.DirectionEnum Direction;
        public RdlEngine.Style.WritingModeEnum WritingMode;
        public string Language;
        public RdlEngine.Style.UnicodeBiDiEnum UnicodeBiDi;
        public string Calendar;
        public string NumeralLanguage;
        public int NumeralVariant;

        public TextStyle(RdlEngine.Style style)
            :base(style)
        {
            if (style == null)
                style = RdlEngine.Style.DefaultStyle;
            FontStyle = style.FontStyle;
            FontFamily = style.FontFamily;
            FontSize = style.FontSize;
            FontWeight = style.FontWeight;
            Format = style.Format;
            TextDecoration = style.TextDecoration;
            TextAlign = style.TextAlign;
            VerticalAlign = style.VerticalAlign;
            LineHeight = style.LineHeight;
            Direction = style.Direction;
            WritingMode = style.WritingMode;
            Language = style.Language;
            UnicodeBiDi = style.UnicodeBiDi;
            Calendar = style.Calendar;
            NumeralLanguage = style.NumeralLanguage;
            NumeralVariant = style.NumeralVariant;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TextStyle))
                return false;
            TextStyle ts = obj as TextStyle;

            return ( base.Equals(ts) &&
                FontStyle.Equals(ts.FontStyle) &&
                FontFamily.Equals(ts.FontFamily) &&
                FontSize.Equals(ts.FontSize) &&
                FontWeight.Equals(ts.FontWeight) &&
                Format.Equals(ts.Format) &&
                TextDecoration.Equals(ts.TextDecoration) &&
                TextAlign.Equals(ts.TextAlign) &&
                VerticalAlign.Equals(ts.VerticalAlign) &&
                LineHeight.Equals(ts.LineHeight) &&
                Direction.Equals(ts.Direction) &&
                WritingMode.Equals(ts.WritingMode) &&
                Language.Equals(ts.Language) &&
                UnicodeBiDi.Equals(ts.UnicodeBiDi) &&
                Calendar.Equals(ts.Calendar) &&
                NumeralLanguage.Equals(ts.NumeralLanguage) &&
                NumeralVariant.Equals(ts.NumeralVariant));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
