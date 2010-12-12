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
