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
using Rdl.Engine;

namespace Rdl.Engine.Table
{
    class Footer : TableElement
    {
        private List<Row> _tableRows = new List<Row>();
        private bool _repeatOnNewPage = false;

        public Footer(XmlNode node, ReportElement parent)
            : base(node, parent)
        {
        }

        protected override void ParseAttribute(XmlNode attr)
        {
            switch (attr.Name.ToLower())
            {
                case "tablerows":
                    foreach(XmlNode child in attr.ChildNodes)
                        _tableRows.Add(new Row(child, this));
                    break;
                case "repeatonnewpage":
                    _repeatOnNewPage = bool.Parse(attr.InnerText);
                    break;
            }
        }

        public bool RepeatOnNewPage
        {
            get { return _repeatOnNewPage; }
        }


        internal override void Render(Rdl.Render.Container box, Rdl.Runtime.Context context)
        {
            foreach (Row row in _tableRows)
                row.Render(box, context);
        }
    }
}
