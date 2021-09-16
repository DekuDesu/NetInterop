using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NetInterop.Transport.Core.Delegates;

namespace NetInterop.Transport.Core.Packets.Extensions
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

        public static string ToString(this Span<byte> buffer, Encoding encoding = default)
        {
            return encoding.GetString(buffer);
        }

        public static Span<byte> ToSpan(this string value, Encoding encoding = default)
        {
            encoding ??= Encoding.UTF8;

            return encoding.GetBytes(value);
        }

        public static DateTime ToDateTime(this Span<byte> buffer)
        {
            return DateTime.FromBinary(buffer.ToLong());
        }

        public static Span<byte> ToSpan(this DateTime value)
        {
            return value.ToBinary().ToSpan();
        }

        public unsafe static Span<byte> ToFullyQualifiedSpan<T>(this T unmanagedObject) where T : unmanaged
        {
            Span<byte> result = new byte[sizeof(T) + 1];

            result[0] = (byte)Type.GetTypeCode(typeof(T));

            // i had a more elegant way of doing this, but this was 75% faster... :(
            if (unmanagedObject is bool a)
            {
                a.ToSpan().CopyTo(result.Slice(1));
            }
            else
            if (unmanagedObject is sbyte b)
            {
                b.ToSpan().CopyTo(result.Slice(1));
            }
            else
            if (unmanagedObject is byte c)
            {
                c.ToSpan().CopyTo(result.Slice(1));
            }
            else
            if (unmanagedObject is short d)
            {
                d.ToSpan().CopyTo(result.Slice(1));
            }
            else
            if (unmanagedObject is ushort e)
            {
                e.ToSpan().CopyTo(result.Slice(1));
            }
            else
            if (unmanagedObject is int f)
            {
                f.ToSpan().CopyTo(result.Slice(1));
            }
            else
            if (unmanagedObject is uint g)
            {
                g.ToSpan().CopyTo(result.Slice(1));
            }
            else
            if (unmanagedObject is long h)
            {
                h.ToSpan().CopyTo(result.Slice(1));
            }
            else
            if (unmanagedObject is ulong i)
            {
                i.ToSpan().CopyTo(result.Slice(1));
            }
            else
            if (unmanagedObject is float j)
            {
                j.ToSpan().CopyTo(result.Slice(1));
            }
            else
            if (unmanagedObject is double k)
            {
                k.ToSpan().CopyTo(result.Slice(1));
            }
            else
            if (unmanagedObject is decimal l)
            {
                l.ToSpan().CopyTo(result.Slice(1));
            }
            else
            if (unmanagedObject is DateTime m)
            {
                m.ToSpan().CopyTo(result.Slice(1));
            }

            return result;
        }

        public static object FromFullyQualifiedSpan<T>(this Span<byte> buffer)
        {
            TypeCode type = (TypeCode)buffer[0];

            buffer = buffer.Slice(1);

            switch (type)
            {
                case TypeCode.Boolean:
                    return buffer.ToBool();
                case TypeCode.Char:
                    return (char)buffer.ToUShort();
                case TypeCode.SByte:
                    return (sbyte)buffer[0];
                case TypeCode.Byte:
                    return buffer[0];
                case TypeCode.Int16:
                    return buffer.ToShort();
                case TypeCode.UInt16:
                    return buffer.ToUShort();
                case TypeCode.Int32:
                    return buffer.ToInt();
                case TypeCode.UInt32:
                    return buffer.ToUInt();
                case TypeCode.Int64:
                    return buffer.ToLong();
                case TypeCode.UInt64:
                    return buffer.ToULong();
                case TypeCode.Single:
                    return buffer.ToFloat();
                case TypeCode.Double:
                    return buffer.ToDouble();
                case TypeCode.Decimal:
                    return buffer.ToDecimal();
                case TypeCode.DateTime:
                    return buffer.ToDateTime();
                case TypeCode.String:
                case TypeCode.Empty:
                case TypeCode.Object:
                case TypeCode.DBNull:
                default:
                    throw new InvalidOperationException($"Failed to convert non-managed type {type} to object.");
            }
        }

        public static Span<byte> ToSpan<T>(this T[] value, Span<byte> buffer, int sizeOfT, ToSpanFunc<T, byte> toSpanConverter) where T : unmanaged
        {
            int index = 0;
            for (int i = 0; i < buffer.Length; i += 4)
            {
                toSpanConverter(value[index++]).CopyTo(buffer.Slice(i, sizeOfT));
            }

            return buffer;
        }

        public static T[] ToArray<T>(this Span<byte> buffer, int sizeOfT, FromSpanFunc<byte, T> spanToObjectConverter)
        {
            T[] result = new T[buffer.Length / sizeOfT];

            int index = 0;
            for (int i = 0; i < buffer.Length; i += sizeOfT)
            {
                result[index++] = spanToObjectConverter(buffer.Slice(i, sizeOfT));
            }

            return result;
        }
    }
}
