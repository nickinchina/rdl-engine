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

namespace Rdl.Engine
{
    class Utility
    {
        static internal int ApplyCompare(object left, object right)
        {
            if (left is string)
                return ((string)left).CompareTo(Convert.ToString(right));
            if (left is decimal)
                return ((Decimal)left).CompareTo(Convert.ToDecimal(right, NumberFormatInfo.InvariantInfo));
            if (left is Single)
                return ((float)left).CompareTo(Convert.ToSingle(right, NumberFormatInfo.InvariantInfo));
            if (left is double)
                return ((double)left).CompareTo(Convert.ToDouble(right, NumberFormatInfo.InvariantInfo));
            if (left is DateTime)
                return ((DateTime)left).CompareTo(Convert.ToDateTime(right));
            if (left is short)
                return ((short)left).CompareTo(Convert.ToInt16(right));
            if (left is ushort)
                return ((ushort)left).CompareTo(Convert.ToUInt16(right));
            if (left is int)
                return ((int)left).CompareTo(Convert.ToInt32(right));
            if (left is uint)
                return ((uint)left).CompareTo(Convert.ToUInt32(right));
            if (left is long)
                return ((long)left).CompareTo(Convert.ToInt64(right));
            if (left is ulong)
                return ((ulong)left).CompareTo(Convert.ToUInt64(right));
            if (left is bool)
                return ((bool)left).CompareTo(Convert.ToBoolean(right));
            if (left is char)
                return ((char)left).CompareTo(Convert.ToChar(right));
            if (left is sbyte)
                return ((sbyte)left).CompareTo(Convert.ToSByte(right));
            if (left is byte)
                return ((byte)left).CompareTo(Convert.ToByte(right));
            if (left is DBNull)
            {
                if (right == null || right is DBNull)
                    return 0;
                else
                    return -1;
            }
            return 0;
        }
    }
}
