using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Pdf
{
    public class PdfObject
    {
        private int _id;
        public long Position;

        public PdfObject(Document doc)
        {
            _id = doc.NextID();
            doc.Objects.Add(this);
        }

        public int Id
        {
            get { return _id; }
        }

    }
}
