using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Render
{
    public class ChartElement : Element
    {
        private Rdl.Runtime.Context _context;

        internal ChartElement(Container parent, Rdl.Engine.ReportElement reportElement, BoxStyle style, Rdl.Runtime.Context context)
            : base(parent, reportElement, style)
        {
            _context = context;
        }

        public ChartElement(ChartElement b) : base(b)
        {
        }


        public Rdl.Runtime.Context Context
        {
            get { return _context; }
        }

        public System.Drawing.Image RenderChart(int width, int height, decimal xMult, decimal yMult)
        {
            return ((Rdl.Engine.Chart.Chart)_reportElement).RenderChart(_context, width, height, xMult, yMult);
        }
    }
}
