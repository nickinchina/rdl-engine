using System;
using System.Collections.Generic;
using System.Text;

namespace RdlRender
{
    // A FixedCOntainer positions child elements at fixed
    // positions.
    public class FixedContainer : Container
    {
        internal FixedContainer(Container parent, RdlEngine.ReportElement reportElement, BoxStyle style)
            : base(parent, reportElement, style)
        {
        }

        public FixedContainer(Container b) : base(b)
        {
        }

        public FixedContainer(Container b, bool copyChildren)
            : base(b, copyChildren)
        {
        }

    }
}
