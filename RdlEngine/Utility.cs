using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RdlEngine
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
                if (right == null)
                    return 0;
                else
                    return -1;
            }
            return 0;
        }
    }
}
