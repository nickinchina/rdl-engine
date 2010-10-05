using System;
using System.Collections.Generic;
using System.Text;

namespace RdlRender
{
    public class BoxStyle
    {
        public BorderColor BorderColor;
        public BorderStyle BorderStyle;
        public BorderWidth BorderWidth;
        public string BackgroundColor;
        public RdlEngine.Style.GradientTypeEnum BackgroundGradientType;
        public string BackgroundGradientEndColor;
        public byte[] BackgroundImage;
        public string BackgroundImageType;
        public string Color;
        public RdlEngine.Size PaddingLeft;
        public RdlEngine.Size PaddingRight;
        public RdlEngine.Size PaddingTop;
        public RdlEngine.Size PaddingBottom;

        public BoxStyle(RdlEngine.Style style)
        {
            if (style == null)
                style = RdlEngine.Style.DefaultStyle;
            BorderColor = new BorderColor(style.BorderColor);
            BorderStyle = new BorderStyle(style.BorderStyle);
            BorderWidth = new BorderWidth(style.BorderWidth);
            BackgroundColor = style.BackgroundColor;
            BackgroundGradientType = style.BackgroundGradientType;
            BackgroundGradientEndColor = style.BackgroundGradientEndColor;
            BackgroundImage = style.BackgroundImage.Value;
            BackgroundImageType = style.BackgroundImage.MIMEType;
            Color = style.Color;
            PaddingLeft = style.PaddingLeft;
            PaddingRight = style.PaddingRight;
            PaddingTop = style.PaddingTop;
            PaddingBottom = style.PaddingBottom;
        }

        public override bool Equals(object obj)
        {
            BoxStyle bs = obj as BoxStyle;

            return (
                BorderColor.Equals(bs.BorderColor) &&
                BorderStyle.Equals(bs.BorderStyle) &&
                BorderWidth.Equals(bs.BorderWidth) &&
                BackgroundColor.Equals(bs.BackgroundColor) &&
                BackgroundGradientType.Equals(bs.BackgroundGradientType) &&
                BackgroundGradientEndColor.Equals(bs.BackgroundGradientEndColor) &&
                BackgroundImage == bs.BackgroundImage &&
                BackgroundImageType.Equals(bs.BackgroundImageType) &&
                Color.Equals(bs.Color) &&
                PaddingLeft.Equals(bs.PaddingLeft) &&
                PaddingRight.Equals(bs.PaddingRight) &&
                PaddingTop.Equals(bs.PaddingTop) &&
                PaddingBottom.Equals(bs.PaddingBottom) );
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
