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
using System.Xml;

namespace Rdl.Engine
{
    class ReportItems : ReportElement
    {
        private List<ReportItem> _reportItems = new List<ReportItem>();

        public ReportItems(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            ReportItem ri = ReportItem.NewReportItem(attr, this);
            if (ri != null)
                _reportItems.Add(ri);
        }

        public List<ReportItem> Items
        {
            get { return _reportItems; }
        }

        internal override void Render(Rdl.Render.Container box,Rdl.Runtime.Context context)
        {
            foreach (ReportItem ri in _reportItems)
            {
                ri.Render(box, context);
            }

            foreach (ReportItem ri in _reportItems)
            {
                if (ri.RepeatWith != null)
                    foreach (ReportItem ri2 in _reportItems)
                        if (ri2.Name == ri.RepeatWith)
                            ri2.Box.RepeatList.Add(ri.Box);
            }
        }
    }
}
