using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteInvoke.Net.Transport.Packets.Extensions
{
    // This might seem counter intuitive over using streams, in my testing this is 50% faster than using streams and has no memory and GC side effects

    public static class RefSpanExtensions
    {
        [Obsolete("Use Packet Instead")]
        public static bool ReadBool(this ref Span<byte> data)
        {
            // slice is faster than indexers as of 9/2021
            bool result = BitConverter.ToBoolean(data.Slice(0, sizeof(bool)));

            data = data.Slice(sizeof(bool));

            return result;
        }
        public static void WriteBool(this ref Span<byte> data, bool value)
        {
            int index = data.Expand(sizeof(bool));

            BitConverter.GetBytes(value).CopyTo(data.Slice(index));
        }

        public static sbyte ReadSByte(this ref Span<byte> data)
        {
            // slice is faster than indexers as of 9/2021
            sbyte result = (sbyte)data.Slice(0, sizeof(sbyte))[0];

            data = data.Slice(sizeof(sbyte));

            return result;
        }
        public static void WriteSByte(this ref Span<byte> data, sbyte value)
        {
            int index = data.Expand(sizeof(sbyte));

            data[index] = (byte)value;
        }

        public static byte ReadByte(this ref Span<byte> data)
        {
            // slice is faster than indexers as of 9/2021
            byte result = (byte)data.Slice(0, sizeof(byte))[0];

            data = data.Slice(sizeof(byte));

            return result;
        }
        public static void WriteByte(this ref Span<byte> data, byte value)
        {
            int index = data.Expand(sizeof(byte));

            data[index] = value;
        }

        public static char ReadChar(this ref Span<byte> data)
        {
            return (char)data.ReadUShort();
        }
        public static void WriteChar(this ref Span<byte> data, char value)
        {
            data.WriteUShort(value);
        }

        public static short ReadShort(this ref Span<byte> data)
        {
            // slice is faster than indexers as of 9/2021
            short result = BitConverter.ToInt16(data.Slice(0, sizeof(short)));

            data = data.Slice(sizeof(short));

            return result;
        }
        public static void WriteShort(this ref Span<byte> data, short value)
        {
            int index = data.Expand(sizeof(short));

            BitConverter.GetBytes(value).CopyTo(data.Slice(index));
        }

        public static ushort ReadUShort(this ref Span<byte> data)
        {
            // slice is faster than indexers as of 9/2021
            ushort result = BitConverter.ToUInt16(data.Slice(0, sizeof(ushort)));

            data = data.Slice(sizeof(ushort));

            return result;
        }
        public static void WriteUShort(this ref Span<byte> data, ushort value)
        {
            int index = data.Expand(sizeof(ushort));

            BitConverter.GetBytes(value).CopyTo(data.Slice(index));
        }

        public static int ReadInt(this ref Span<byte> data)
        {
            // slice is faster than indexers as of 9/2021
            int result = BitConverter.ToInt32(data.Slice(0, sizeof(int)));

            data = data.Slice(sizeof(int));

            return result;
        }
        public static void WriteInt(this ref Span<byte> data, int value)
        {
            int index = data.Expand(sizeof(int));

            BitConverter.GetBytes(value).CopyTo(data.Slice(index));
        }

        public static uint ReadUInt(this ref Span<byte> data)
        {
            // slice is faster than indexers as of 9/2021
            uint result = BitConverter.ToUInt32(data.Slice(0, sizeof(uint)));

            data = data.Slice(sizeof(uint));

            return result;
        }
        public static void WriteUInt(this ref Span<byte> data, uint value)
        {
            int index = data.Expand(sizeof(uint));

            BitConverter.GetBytes(value).CopyTo(data.Slice(index));
        }

        public static long ReadLong(this ref Span<byte> data)
        {
            // slice is faster than indexers as of 9/2021
            long result = BitConverter.ToInt64(data.Slice(0, sizeof(long)));

            data = data.Slice(sizeof(long));

            return result;
        }
        public static void WriteLong(this ref Span<byte> data, long value)
        {
            int index = data.Expand(sizeof(long));

            BitConverter.GetBytes(value).CopyTo(data.Slice(index));
        }

        public static ulong ReadULong(this ref Span<byte> data)
        {
            // slice is faster than indexers as of 9/2021
            ulong result = BitConverter.ToUInt64(data.Slice(0, sizeof(ulong)));

            data = data.Slice(sizeof(ulong));

            return result;
        }
        public static void WriteULong(this ref Span<byte> data, ulong value)
        {
            int index = data.Expand(sizeof(ulong));

            BitConverter.GetBytes(value).CopyTo(data.Slice(index));
        }

        public static float ReadFloat(this ref Span<byte> data)
        {
            // slice is faster than indexers as of 9/2021
            float result = BitConverter.ToSingle(data.Slice(0, sizeof(float)));

            data = data.Slice(sizeof(float));

            return result;
        }
        public static void WriteFloat(this ref Span<byte> data, float value)
        {
            int index = data.Expand(sizeof(float));

            BitConverter.GetBytes(value).CopyTo(data.Slice(index));
        }

        public static double ReadDouble(this ref Span<byte> data)
        {
            // slice is faster than indexers as of 9/2021
            double result = BitConverter.ToDouble(data.Slice(0, sizeof(double)));

            data = data.Slice(sizeof(double));

            return result;
        }
        public static void WriteDouble(this ref Span<byte> data, double value)
        {
            int index = data.Expand(sizeof(double));

            BitConverter.GetBytes(value).CopyTo(data.Slice(index));
        }

        public static decimal ReadDecimal(this ref Span<byte> data)
        {
            decimal value = data.Slice(0, sizeof(decimal)).ToDecimal();

            data = data.Slice(sizeof(decimal));

            return value;
        }
        public static void WriteDecimal(this ref Span<byte> data, decimal value)
        {
            int[] bits = decimal.GetBits(value);

            // this is an intentional violation of DRY, SpanConverterExtensions duplicates this code
            // this uses stackalloc and SpanConverterExtensions uses heap alloc, this is faster for ref spans
            Span<byte> bytes = stackalloc byte[16];

            BitConverter.GetBytes(bits[0]).CopyTo(bytes.Slice(0, 4));
            BitConverter.GetBytes(bits[1]).CopyTo(bytes.Slice(4, 4));
            BitConverter.GetBytes(bits[2]).CopyTo(bytes.Slice(8, 4));
            BitConverter.GetBytes(bits[3]).CopyTo(bytes.Slice(12, 4));

            int index = data.Expand(sizeof(decimal));

            bytes.CopyTo(data.Slice(index));
        }

        public static string ReadString(this ref Span<byte> data, int stringLength, Encoding encoding = default)
        {
            int byteCount = encoding.GetByteCount("a") * stringLength;

            string value = encoding.GetString(data.Slice(0, byteCount));

            data = data.Slice(byteCount);

            return value;
        }
        public static void WriteString(this ref Span<byte> data, string value, Encoding encoding = default)
        {
            encoding ??= Encoding.UTF8;

            Span<byte> bytes = encoding.GetBytes(value);

            int index = data.Expand(bytes.Length);

            bytes.CopyTo(data.Slice(index));
        }

        public static DateTime ReadDateTime(this ref Span<byte> data)
        {
            long binaryDateTime = data.ReadLong();

            return DateTime.FromBinary(binaryDateTime);
        }
        public static void WriteDateTime(this ref Span<byte> data, DateTime value)
        {
            data.WriteLong(value.ToBinary());
        }

        /// <summary>
        /// Expands the provided <see cref="Span{T}"/> returns the index where the additional space starts
        /// </summary>
        /// <param name="data"></param>
        /// <param name="additionalSize"></param>
        /// <returns></returns>
        public static int Expand<T>(this ref Span<T> data, int additionalSize)
        {
            int previousLength = data.Length;

            Span<T> bytes = new T[previousLength + additionalSize];

            data.CopyTo(bytes);

            data = bytes;

            return previousLength;
        }

        /// <summary>
        /// Expands the provided <see cref="Span{T}"/> returns the index where the additional space starts
        /// </summary>
        /// <param name="data"></param>
        /// <param name="additionalSize"></param>
        /// <returns></returns>
        public static void ExpandLeft<T>(this ref Span<T> data, int additionalSize)
        {
            Span<T> bytes = new T[data.Length + additionalSize];

            data.CopyTo(bytes.Slice(additionalSize));

            data = bytes;
        }
    }
}
