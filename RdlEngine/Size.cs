using System;
using System.Collections.Generic;
using System.Text;

namespace RdlEngine
{
    public class Size 
    {
        // Size is stored in points
        decimal _size;
        public static Size ZeroSize = new Size("0in");
        public static Size OnePt = new Size("1pt");

        public Size(decimal size)
        {
            _size = size;
        }

        public Size(string size)
        {
            int i=0;
            for (i = 0; i < size.Length; i++)
                if ((size[i] < '0' || size[i] > '9') &&
                    size[i] != ' ' &&
                    size[i] != '.' &&
                    size[i] != '+' &&
                    size[i] != '-')
                    break;

            decimal sz = decimal.Parse(size.Substring(0, i));
            string tp = "in";
            if (i < size.Length)
                tp = size.Substring(i);
            switch (tp)
            {
                case "in":
                    _size = sz * 72m;
                    break;
                case "cm":
                    _size = sz * 28.35m;
                    break;
                case "mm":
                    _size = sz * 2.835m;
                    break;
                case "pt":
                    _size = sz;
                    break;
                case "pc":
                    _size = sz * 12m;
                    break;
            }
        }

        public decimal points
        {
            get { return _size; }
        }

        public decimal inches
        {
            get { return _size / 72m; }
        }

        public decimal cm
        {
            get { return _size / 28.35m; }
        }

        public decimal mm
        {
            get { return _size / 2.835m; }
        }

        public decimal pc
        {
            get { return _size / 12m; }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Size))
                return false;
            Size s = obj as Size;

            return (s._size == _size);
        }

        public override int GetHashCode()
        {
            return _size.GetHashCode();
        }
    }
}
