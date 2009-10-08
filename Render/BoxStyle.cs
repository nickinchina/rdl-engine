using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Render
{
    public class BoxStyle
    {
        public BorderColor BorderColor;
        public BorderStyle BorderStyle;
        public BorderWidth BorderWidth;
        public string BackgroundColor;
        public Rdl.Engine.Style.GradientTypeEnum BackgroundGradientType;
        public string BackgroundGradientEndColor;
        public ImageData BackgroundImage = null;
        //public Rdl.Engine.BackgroundImage BackgroundImage;
        public string Color;
        public Rdl.Engine.Size PaddingLeft;
        public Rdl.Engine.Size PaddingRight;
        public Rdl.Engine.Size PaddingTop;
        public Rdl.Engine.Size PaddingBottom;

        public BoxStyle(Rdl.Engine.Style style, Rdl.Runtime.Context context)
        {
            if (style == null)
                style = Rdl.Engine.Style.DefaultStyle;
            BorderColor = new BorderColor(style.BorderColor, context);
            BorderStyle = new BorderStyle(style.BorderStyle, context);
            BorderWidth = new BorderWidth(style.BorderWidth, context);
            BackgroundColor = style.BackgroundColor(context);
            BackgroundGradientType = style.BackgroundGradientType(context);
            BackgroundGradientEndColor = style.BackgroundGradientEndColor(context);
            Color = style.Color(context);
            PaddingLeft = style.PaddingLeft(context);
            PaddingRight = style.PaddingRight(context);
            PaddingTop = style.PaddingTop(context);
            PaddingBottom = style.PaddingBottom(context);

            Rdl.Engine.BackgroundImage bi = style.BackgroundImage;
            if (bi != null)
            {
                BackgroundImage = new ImageData();
                BackgroundImage.imageData = bi.GetImage(context);
                BackgroundImage.Sizing = bi.Sizing;
                BackgroundImage.ImageRepeat = bi.ImageRepeat(context);
            }
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

        //public string BackgroundImageType
        //{
        //    get
        //    {
        //        if (BackgroundImage != null)
        //            return BackgroundImage.MIMEType;
        //        else
        //            return null;
        //    }
        //}
    }
}
