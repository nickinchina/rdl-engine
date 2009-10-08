using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Drawing;

namespace Rdl.Pdf
{
    public class PageTree : PdfObject
    {
        public List<PageTree> Trees = new List<PageTree>();
        public List<Page> Pages = new List<Page>();

        public PageTree(Document doc) : base(doc)
        {
        }

        public Page AddPage(Document doc, Rectangle rect)
        {
            Page p = new Page(doc, this, rect);
            Pages.Add(p);
            return p;
        }

        public override string ToString()
        {
            MyStringBuilder sb = new MyStringBuilder();
            sb.Append(1, "<</Type /Pages");
            sb.Append(2, "/Kids [");
            foreach (PageTree pt in Trees)
                sb.Append(3, pt.Id.ToString() + " 0 R");
            foreach (Page p in Pages)
                sb.Append(3, p.Id.ToString() + " 0 R");
            sb.Append(3, "]");
            sb.Append(2, "/Count " + (Trees.Count + Pages.Count).ToString());
            sb.Append(1, ">>");
            return sb.ToString();
        }
    }
}
