using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteInvoke.Net.Transport.Packets.Extensions
{
    /// <summary>
    /// Converts unmanaged types to and from Span<byte>
    /// </summary>
    public static class SpanTypeConversionExtensions
    {
        public static bool ToBool(this Span<byte> buffer)
        {
            return BitConverter.ToBoolean(buffer);
        }

        public static sbyte ToSByte(this Span<byte> buffer)
        {
            if (buffer.Length != 1)
            {
                return 0;
            }
            return (sbyte)buffer.Slice(1)[0];
        }

        public static byte ToByte(this Span<byte> buffer)
        {
            if (buffer.Length != 1)
            {
                return 0;
            }
            return (byte)buffer.Slice(1)[0];
        }

        public static short ToShort(this Span<byte> buffer)
        {
            return BitConverter.ToInt16(buffer);
        }

        public static ushort ToUShort(this Span<byte> buffer)
        {
            return BitConverter.ToUInt16(buffer);
        }

        public static int ToInt(this Span<byte> buffer)
        {
            return BitConverter.ToInt32(buffer);
        }

        public static uint ToUInt(this Span<byte> buffer)
        {
            return BitConverter.ToUInt32(buffer);
        }

        public static long ToLong(this Span<byte> buffer)
        {
            return BitConverter.ToInt64(buffer);
        }

        public static ulong ToULong(this Span<byte> buffer)
        {
            return BitConverter.ToUInt64(buffer);
        }

        public static float ToFloat(this Span<byte> buffer)
        {
            return BitConverter.ToSingle(buffer);
        }

        public static double ToDouble(this Span<byte> buffer)
        {
            return BitConverter.ToDouble(buffer);
        }

        public static decimal ToDecimal(this Span<byte> buffer)
        {
            // https://docs.microsoft.com/en-us/dotnet/api/system.decimal.getbits?view=net-5.0

            int low = BitConverter.ToInt32(buffer.Slice(0, 4));

            int mid = BitConverter.ToInt32(buffer.Slice(4, 4));

            int high = BitConverter.ToInt32(buffer.Slice(8, 4));

            int scaleAndSign = BitConverter.ToInt32(buffer.Slice(12, 4));

            bool sign = (scaleAndSign & 0x80000000) != 0;

            byte scale = (byte)(scaleAndSign >> 16 & 0x7F);

            return new decimal(low, mid, high, sign, scale);
        }

        public static Span<byte> ToSpan(this bool value)
        {
            return BitConverter.GetBytes(value);
        }

        public static Span<byte> ToSpan(this sbyte value)
        {
            return BitConverter.GetBytes(value);
        }

        public static Span<byte> ToSpan(this byte value)
        {
            return BitConverter.GetBytes(value);
        }

        public static Span<byte> ToSpan(this short value)
        {
            return BitConverter.GetBytes(value);
        }

        public static Span<byte> ToSpan(this ushort value)
        {
            return BitConverter.GetBytes(value);
        }

        public static Span<byte> ToSpan(this int value)
        {
            return BitConverter.GetBytes(value);
        }

        public static Span<byte> ToSpan(this uint value)
        {
            return BitConverter.GetBytes(value);
        }

        public static Span<byte> ToSpan(this long value)
        {
            return BitConverter.GetBytes(value);
        }

        public static Span<byte> ToSpan(this ulong value)
        {
            return BitConverter.GetBytes(value);
        }

        public static Span<byte> ToSpan(this float value)
        {
            return BitConverter.GetBytes(value);
        }

        public static Span<byte> ToSpan(this double value)
        {
            return BitConverter.GetBytes(value);
        }

        public static Span<byte> ToSpan(this decimal value)
        {
            int[] bits = decimal.GetBits(value);

            Span<byte> bytes = new byte[16];

            BitConverter.GetBytes(bits[0]).CopyTo(bytes.Slice(0, 4));
            BitConverter.GetBytes(bits[1]).CopyTo(bytes.Slice(4, 4));
            BitConverter.GetBytes(bits[2]).CopyTo(bytes.Slice(8, 4));
            BitConverter.GetBytes(bits[3]).CopyTo(bytes.Slice(12, 4));

            return bytes;
        }

    }
}
