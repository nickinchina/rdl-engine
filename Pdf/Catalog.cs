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
using System.Globalization;

namespace Rdl.Pdf
{
    public class Catalog : PdfObject
    {
        PageTree _pageTree;

        public Catalog(Document doc, PageTree pageTree)
            : base(doc)
        {
            _pageTree = pageTree;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<</Type /Catalog");
            sb.AppendFormat(NumberFormatInfo.InvariantInfo,
                "/Pages {0} 0 R\n", _pageTree.Id);
            sb.AppendLine("/PageMode /UseNone>>");

            return sb.ToString();
        }
    }
}
