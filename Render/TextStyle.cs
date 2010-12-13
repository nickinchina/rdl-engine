/*-----------------------------------------------------------------------------------
This file is part of the SawikiSoft RDL Engine.
The SawikiSoft RDL Engine is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

The SawikiSoft RDL Engine is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
-----------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Rdl.Render
{
    public class TextStyle : BoxStyle
    {
        public Rdl.Engine.Style.FontStyleEnum FontStyle;
        public string FontFamily;
        public Rdl.Engine.Size FontSize;
        public Rdl.Engine.Style.FontWeightEnum FontWeight;
        public string Format;
        public Rdl.Engine.Style.TextDecorationEnum TextDecoration;
        public Rdl.Engine.Style.TextAlignEnum TextAlign;
        public Rdl.Engine.Style.VerticalAlignEnum VerticalAlign;
        public Rdl.Engine.Size LineHeight;
        public Rdl.Engine.Style.DirectionEnum Direction;
        public Rdl.Engine.Style.WritingModeEnum WritingMode;
        public string Language;
        public Rdl.Engine.Style.UnicodeBiDiEnum UnicodeBiDi;
        public string Calendar;
        public string NumeralLanguage;
        public int NumeralVariant;

        public TextStyle(Rdl.Engine.Style style, Rdl.Runtime.Context context)
            : base(style, context)
        {
            if (style == null)
                style = Rdl.Engine.Style.DefaultStyle;
            FontStyle = style.FontStyle(context);
            FontFamily = style.FontFamily(context);
            FontSize = style.FontSize(context);
            FontWeight = style.FontWeight(context);
            Format = style.Format(context);
            TextDecoration = style.TextDecoration(context);
            TextAlign = style.TextAlign(context);
            VerticalAlign = style.VerticalAlign(context);
            LineHeight = style.LineHeight(context);
            Direction = style.Direction(context);
            WritingMode = style.WritingMode(context);
            Language = style.Language(context);
            UnicodeBiDi = style.UnicodeBiDi(context);
            Calendar = style.Calendar(context);
            NumeralLanguage = style.NumeralLanguage(context);
            NumeralVariant = style.NumeralVariant(context);
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

        public System.Drawing.Font GetWindowsFont()
        {
            System.Drawing.FontStyle fs = (FontStyle == Rdl.Engine.Style.FontStyleEnum.Italic) ? System.Drawing.FontStyle.Italic : System.Drawing.FontStyle.Regular;
            if (FontWeight >= Rdl.Engine.Style.FontWeightEnum.Bold)
                fs |= System.Drawing.FontStyle.Bold;
            if (TextDecoration == Rdl.Engine.Style.TextDecorationEnum.Underline)
                fs |= System.Drawing.FontStyle.Underline;
            if (TextDecoration == Rdl.Engine.Style.TextDecorationEnum.LineThrough)
                fs |= System.Drawing.FontStyle.Strikeout;
            return new Font(FontFamily, (float)FontSize.points, fs, GraphicsUnit.Point);
        }
    }
}
