using System;
using System.Collections.Generic;
using System.Text;

namespace RdlRender
{
    class ImageElement : Element
    {
        public ImageElement(Container parent, RdlEngine.ReportItem reportItem, BoxStyle style)
            : base(parent, reportItem, style)
        {
        }

        public ImageElement(ImageElement ie)
            : base(ie)
        {
        }
    }
}
