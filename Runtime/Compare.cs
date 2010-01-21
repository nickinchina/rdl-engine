using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Runtime
{
    class Compare
    {
        public static int CompareTo(object obj1, object obj2)
        {
            if (Convert.IsDBNull(obj1))
                if (Convert.IsDBNull(obj2))
                    return 0;
                else
                    return -1;
            else
                if (Convert.IsDBNull(obj2))
                    return 1;
                else
                    return ((IComparable)obj1).CompareTo(obj2);
        }
    }
}
