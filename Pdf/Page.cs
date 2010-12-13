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
using System.Drawing;
using System.Globalization;

namespace Rdl.Pdf
{
    public class Page : PdfObject
    {
        private PageTree _parent;
        private Rectangle _pageRect;
        private List<ContentStream> _contents = new List<ContentStream>();
        private StringBuilder _annotations = new StringBuilder();

        public Page(Document doc, PageTree pt, Rectangle pageRect) : base(doc)
        {
            _parent = pt;
            _pageRect = pageRect;
        }

        public override string ToString()
        {
            MyStringBuilder sb = new MyStringBuilder();
            sb.Append(1, "<</Type /Page");
            sb.Append(2, "/Parent " + _parent.Id.ToString() + " 0 R");
            StringBuilder fonts = new StringBuilder();
            foreach (ContentStream cs in _contents)
                foreach (Font f in cs.FontsUsed.Values)
                    fonts.AppendFormat(NumberFormatInfo.InvariantInfo,
                        "/{0} {1} 0 R ", f.Name, f.Id);
            sb.Append(2, string.Format("/Resources<</Font<<{0}>>/ProcSet[/PDF/Text]>>", fonts));
            if (_annotations.Length > 0)
                sb.Append(2, "/Annots [" + _annotations.ToString() + "]");
            sb.Append(2, string.Format("/MediaBox [{0} {1} {2} {3}]",
                _pageRect.Left, _pageRect.Top,
                _pageRect.Left + _pageRect.Width,
                _pageRect.Top + _pageRect.Height));
            sb.Append(2, "/Rotate 0");
            sb.Append(2, "/Contents [");
            for (int i = 0; i < _contents.Count; i++)
                sb.Append(3, _contents[i].Id.ToString() + " 0 R");
            sb.Append(3, "]");
            sb.Append(1, ">>");
            return sb.ToString();
        }

        public ContentStream AddContents(Document doc)
        {
            ContentStream cs = new ContentStream(doc, this);
            _contents.Add(cs);
            return cs;
        }

        public StringBuilder Annotations
        {
            get { return _annotations; }
        }

    }
}
