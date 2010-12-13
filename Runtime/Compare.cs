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
