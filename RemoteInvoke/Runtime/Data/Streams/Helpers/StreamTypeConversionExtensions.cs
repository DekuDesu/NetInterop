using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace RemoteInvoke.Runtime.Data.Helpers
{
    public static class StreamTypeConversionExtensions
    {
        public delegate int SpanReadFunc(Span<byte> buffer);
        public delegate void SpanWriteFunc(ReadOnlySpan<byte> buffer);

        public static bool ReadBool(this IStream<byte> stream) => ReadBool(stream.Read);

        public static bool ReadBool(this Stream stream) => ReadBool(stream.Read);

        private static bool ReadBool(SpanReadFunc Expression)
        {
            Span<byte> buffer = new byte[1];

            int read = Expression(buffer);

            if (read != 1)
            {
                throw new ArgumentOutOfRangeException($"Failed to read bool from stream, {read} bytes read, expected 1");
            }

            return BitConverter.ToBoolean(buffer);
        }

        public static short ReadShort(this Stream stream) => ReadShort(stream.Read);

        public static short ReadShort(this IStream<byte> stream) => ReadShort(stream.Read);

        private static short ReadShort(SpanReadFunc stream)
        {
            Span<byte> buffer = new byte[2];

            int read = stream(buffer);

            if (read != 2)
            {
                throw new ArgumentOutOfRangeException($"Failed to read short from stream, only {read} bytes read, expected 2");
            }

            return BitConverter.ToInt16(buffer);
        }

        public static ushort ReadUShort(this Stream stream) => ReadUShort(stream.Read);

        public static ushort ReadUShort(this IStream<byte> stream) => ReadUShort(stream.Read);

        private static ushort ReadUShort(SpanReadFunc stream)
        {
            Span<byte> buffer = new byte[2];

            int read = stream(buffer);

            if (read != 2)
            {
                throw new ArgumentOutOfRangeException($"Failed to read short from stream, only {read} bytes read, expected 2");
            }

            return BitConverter.ToUInt16(buffer);
        }

        public static int ReadInt(this Stream stream) => ReadInt(stream.Read);

        public static int ReadInt(this IStream<byte> stream) => ReadInt(stream.Read);

        private static int ReadInt(SpanReadFunc stream)
        {
            Span<byte> buffer = new byte[4];

            int read = stream(buffer);

            if (read != 4)
            {
                throw new ArgumentOutOfRangeException($"Failed to read int from stream, only {read} bytes read, expected 4");
            }

            return BitConverter.ToInt32(buffer);
        }

        public static uint ReadUInt(this Stream stream) => ReadUInt(stream.Read);

        public static uint ReadUInt(this IStream<byte> stream) => ReadUInt(stream.Read);

        private static uint ReadUInt(SpanReadFunc stream)
        {
            Span<byte> buffer = new byte[4];

            int read = stream(buffer);

            if (read != 4)
            {
                throw new ArgumentOutOfRangeException($"Failed to read int from stream, only {read} bytes read, expected 4");
            }

            return BitConverter.ToUInt32(buffer);
        }

        public static long ReadLong(this Stream stream) => ReadLong(stream.Read);

        public static long ReadLong(this IStream<byte> stream) => ReadLong(stream.Read);

        private static long ReadLong(SpanReadFunc stream)
        {
            Span<byte> buffer = new byte[8];

            int read = stream(buffer);

            if (read != 8)
            {
                throw new ArgumentOutOfRangeException($"Failed to read long from stream, only {read} bytes read, expected 8");
            }

            return BitConverter.ToInt64(buffer);
        }

        public static ulong ReadULong(this Stream stream) => ReadULong(stream.Read);

        public static ulong ReadULong(this IStream<byte> stream) => ReadULong(stream.Read);

        private static ulong ReadULong(SpanReadFunc stream)
        {
            Span<byte> buffer = new byte[8];

            int read = stream(buffer);

            if (read != 8)
            {
                throw new ArgumentOutOfRangeException($"Failed to read long from stream, only {read} bytes read, expected 8");
            }

            return BitConverter.ToUInt64(buffer);
        }

        public static string ReadString(this Stream stream, int length, Encoding encoding) => ReadString(stream.Read, length, encoding);

        public static string ReadString(this IStream<byte> stream, int length, Encoding encoding) => ReadString(stream.Read, length, encoding);

        private static string ReadString(SpanReadFunc stream, int length, Encoding encoding)
        {
            Span<byte> buffer = new byte[length];

            int read = stream(buffer);

            if (read != length)
            {
                throw new ArgumentOutOfRangeException($"Failed to read string from stream, only {read} bytes read, expected {length}");
            }

            return encoding.GetString(buffer);
        }

        public static float ReadFloat(this Stream stream) => ReadFloat(stream.Read);

        public static float ReadFloat(this IStream<byte> stream) => ReadFloat(stream.Read);

        private static float ReadFloat(SpanReadFunc stream)
        {
            Span<byte> buffer = new byte[4];

            int read = stream(buffer);

            if (read != 4)
            {
                throw new ArgumentOutOfRangeException($"Failed to read float from stream, only {read} bytes read, expected {8}");
            }

            return BitConverter.ToSingle(buffer);
        }

        public static double ReadDouble(this Stream stream) => ReadDouble(stream.Read);

        public static double ReadDouble(this IStream<byte> stream) => ReadDouble(stream.Read);

        private static double ReadDouble(SpanReadFunc stream)
        {
            Span<byte> buffer = new byte[8];

            int read = stream(buffer);

            if (read != 8)
            {
                throw new ArgumentOutOfRangeException($"Failed to read double from stream, only {read} bytes read, expected {8}");
            }

            return BitConverter.ToDouble(buffer);
        }

        public static decimal ReadDecimal(this Stream stream) => ReadDecimal(stream.Read);

        public static decimal ReadDecimal(this IStream<byte> stream) => ReadDecimal(stream.Read);

        private static decimal ReadDecimal(SpanReadFunc stream)
        {
            Span<byte> buffer = new byte[16];

            int read = stream(buffer);

            if (read != 16)
            {
                throw new ArgumentOutOfRangeException($"Failed to read decimal from stream, only {read} bytes read, expected {16}");
            }

            // https://docs.microsoft.com/en-us/dotnet/api/system.decimal.getbits?view=net-5.0

            int low = BitConverter.ToInt32(buffer.Slice(0, 4));

            int mid = BitConverter.ToInt32(buffer.Slice(4, 4));

            int high = BitConverter.ToInt32(buffer.Slice(8, 4));

            int scaleAndSign = BitConverter.ToInt32(buffer.Slice(12, 4));

            bool sign = (scaleAndSign & 0x80000000) != 0;

            byte scale = (byte)((scaleAndSign >> 16) & 0x7F);

            return new decimal(low, mid, high, sign, scale);
        }

        public static void WriteDecimal(this Stream stream, decimal value) => WriteDecimal(stream.Write, value);

        public static void WriteDecimal(this IStream<byte> stream, decimal value) => WriteDecimal(stream.Write, value);

        private static void WriteDecimal(SpanWriteFunc stream, decimal value)
        {
            int[] bits = decimal.GetBits(value);

            Span<byte> bytes = new byte[16];

            BitConverter.GetBytes(bits[0]).CopyTo(bytes.Slice(0, 4));
            BitConverter.GetBytes(bits[1]).CopyTo(bytes.Slice(4, 4));
            BitConverter.GetBytes(bits[2]).CopyTo(bytes.Slice(8, 4));
            BitConverter.GetBytes(bits[3]).CopyTo(bytes.Slice(12, 4));

            stream(bytes);
        }

        public static void WriteString(this Stream stream, string value, Encoding encoding) => WriteString(stream.Write, value, encoding);

        public static void WriteString(this IStream<byte> stream, string value, Encoding encoding) => WriteString(stream.Write, value, encoding);

        private static void WriteString(SpanWriteFunc stream, string value, Encoding encoding)
        {
            stream(encoding.GetBytes(value));
        }

        public static void WriteBool(this Stream stream, bool value) => WriteBool(stream.Write, value);

        public static void WriteBool(this IStream<byte> stream, bool value) => WriteBool(stream.Write, value);

        private static void WriteBool(SpanWriteFunc stream, bool value)
        {
            stream(BitConverter.GetBytes(value));
        }

        public static void WriteShort(this Stream stream, short value) => WriteShort(stream.Write, value);

        public static void WriteShort(this IStream<byte> stream, short value) => WriteShort(stream.Write, value);

        private static void WriteShort(SpanWriteFunc stream, short value)
        {
            stream(BitConverter.GetBytes(value));
        }

        public static void WriteUShort(this Stream stream, ushort value) => WriteUShort(stream.Write, value);

        public static void WriteUShort(this IStream<byte> stream, ushort value) => WriteUShort(stream.Write, value);

        private static void WriteUShort(SpanWriteFunc stream, ushort value)
        {
            stream(BitConverter.GetBytes(value));
        }

        public static void WriteInt(this Stream stream, int value) => WriteInt(stream.Write, value);

        public static void WriteInt(this IStream<byte> stream, int value) => WriteInt(stream.Write, value);

        private static void WriteInt(SpanWriteFunc stream, int value)
        {
            stream(BitConverter.GetBytes(value));
        }

        public static void WriteUInt(this Stream stream, uint value) => WriteUInt(stream.Write, value);

        public static void WriteUInt(this IStream<byte> stream, uint value) => WriteUInt(stream.Write, value);

        private static void WriteUInt(SpanWriteFunc stream, uint value)
        {
            stream(BitConverter.GetBytes(value));
        }

        public static void WriteLong(this Stream stream, long value) => WriteLong(stream.Write, value);

        public static void WriteLong(this IStream<byte> stream, long value) => WriteLong(stream.Write, value);

        private static void WriteLong(SpanWriteFunc stream, long value)
        {
            stream(BitConverter.GetBytes(value));
        }

        public static void WriteULong(this Stream stream, ulong value) => WriteULong(stream.Write, value);

        public static void WriteULong(this IStream<byte> stream, ulong value) => WriteULong(stream.Write, value);

        private static void WriteULong(SpanWriteFunc stream, ulong value)
        {
            stream(BitConverter.GetBytes(value));
        }

        public static void WriteFloat(this Stream stream, float value) => WriteFloat(stream.Write, value);

        public static void WriteFloat(this IStream<byte> stream, float value) => WriteFloat(stream.Write, value);

        private static void WriteFloat(SpanWriteFunc stream, float value)
        {
            stream(BitConverter.GetBytes(value));
        }

        public static void WriteDouble(this Stream stream, double value) => WriteDouble(stream.Write, value);

        public static void WriteDouble(this IStream<byte> stream, double value) => WriteDouble(stream.Write, value);

        private static void WriteDouble(SpanWriteFunc stream, double value)
        {
            stream(BitConverter.GetBytes(value));
        }
    }
}
