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
