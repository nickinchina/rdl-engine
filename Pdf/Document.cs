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
using System.Globalization;
using System.Text;

namespace Rdl.Pdf
{
    public class Document
    {
        public PageTree Pages;
        private int _idIndex = 0;
        public List<PdfObject> Objects = new List<PdfObject>();

        public Document()
        {
            Pages = new PageTree(this);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            // Build the header
            sb.AppendLine("%PDF-1.5");
            sb.AppendLine("%\x82\x82\x82\x82");

            // Build the catalog
            Catalog catalog = new Catalog(this, Pages);

            // Add in all of the document objects.
            foreach( PdfObject o in Objects)
            {
                o.Position = sb.Length;
                sb.AppendFormat("{0} 0 obj \n", o.Id);
                sb.Append( o.ToString() );
                sb.AppendLine(" endobj");
            }

            // Add in the CrossReference table
            int xRefPos = sb.Length;
            sb.AppendLine("xref");
            sb.AppendLine("1 " + _idIndex.ToString());
            for (int i = 0; i < _idIndex; i++)
            {
                sb.AppendLine(
                    string.Format("{0:0000000000} 00000 n ", Objects[i].Position));
            }

            // Build the trailer
            sb.AppendLine(string.Format("trailer << /Size {0} /Root {1} 0 R>>",
                _idIndex.ToString(), catalog.Id));

            // Build the footer.
            sb.AppendLine("startxref");
            sb.AppendLine(xRefPos.ToString());
            sb.AppendLine("%%EOF");

            return sb.ToString();
        }

        public int NextID()
        {
            return ++_idIndex;
        }
    }
}
