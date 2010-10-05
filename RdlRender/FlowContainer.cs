using System;
using System.Collections.Generic;
using System.Text;

namespace RdlRender
{
    // A FlowContainer flows elements one after another.
    // Elements are always positioned immediately following
    // the previous element and flush left.
    internal class FlowContainer : Container
    {
        internal FlowContainer(Container parent, RdlEngine.ReportElement reportElement, BoxStyle style)
            : base(parent, reportElement, style)
        {
        }

        public FlowContainer(Container b) : base(b)
        {
        }

        public FlowContainer(Container b, bool copyChildren)
            : base(b, copyChildren)
        {
        }
    }
}
