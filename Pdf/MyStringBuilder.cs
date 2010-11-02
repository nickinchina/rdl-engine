using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Rdl.Pdf
{
    class MyStringBuilder 
    {
        StringBuilder sb = new StringBuilder();

        public void Append(int level, string s)
        {
#if DEBUG
            sb.AppendLine(string.Empty.PadRight(level<<1) + s);
#else
            sb.AppendLine(s);
#endif
        }

        public override string  ToString()
        {
 	        return sb.ToString();
        }
    }
}
