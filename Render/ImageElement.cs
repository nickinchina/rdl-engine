using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Render
{
    public class ImageElement : ActionElement
    {
        public ImageElement(Container parent, 
            Rdl.Engine.ReportElement reportItem, 
            BoxStyle style
            )
            : base(parent, reportItem, style)
        {
        }

        public ImageElement(ImageElement ie)
            : base(ie)
        {
        }
    }
}
