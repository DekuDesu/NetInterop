using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RemoteInvoke.Net.Transport.Packets.Extensions;

namespace RemoteInvoke.Net.Transport.Extensions
{
    public static class StreamExtensions
    {
        public static Span<T> Read<T>(this IStream<T> stream, int length)
        {
            Span<T> buffer = new T[length];

            stream.Read(buffer);

            return buffer;
        }

        public static bool ReadBool(this IStream<byte> stream)
        {
            return stream.Read(sizeof(bool)).ToBool();
        }
        public static sbyte ReadSByte(this IStream<byte> stream)
        {
            return stream.Read(sizeof(sbyte)).ToSByte();
        }
        public static byte ReadByte(this IStream<byte> stream)
        {
            return stream.Read(sizeof(byte)).ToByte();
        }
        public static short ReadShort(this IStream<byte> stream)
        {
            return stream.Read(sizeof(short)).ToShort();
        }
        public static ushort ReadUShort(this IStream<byte> stream)
        {
            return stream.Read(sizeof(ushort)).ToUShort();
        }
        public static int ReadInt(this IStream<byte> stream)
        {
            return stream.Read(sizeof(int)).ToInt();
        }
        public static uint ReadUInt(this IStream<byte> stream)
        {
            return stream.Read(sizeof(uint)).ToUInt();
        }
        public static long ReadLong(this IStream<byte> stream)
        {
            return stream.Read(sizeof(long)).ToLong();
        }
        public static ulong ReadULong(this IStream<byte> stream)
        {
            return stream.Read(sizeof(ulong)).ToULong();
        }
        public static float ReadFloat(this IStream<byte> stream)
        {
            return stream.Read(sizeof(float)).ToFloat();
        }
        public static double ReadDouble(this IStream<byte> stream)
        {
            return stream.Read(sizeof(double)).ToDouble();
        }
        public static decimal ReadDecimal(this IStream<byte> stream)
        {
            return stream.Read(sizeof(decimal)).ToDecimal();
        }

        public static string ReadString(this IStream<byte> stream, int length, Encoding encoding = default)
        {
            encoding ??= Encoding.UTF8;

            return encoding.GetString(stream.Read(length));
        }

        public static void WriteBool(this IStream<byte> stream, bool value)
        {
            stream.Write(value.ToSpan());
        }
        public static void WriteSByte(this IStream<byte> stream, sbyte value)
        {
            stream.Write(value.ToSpan());
        }
        public static void WriteByte(this IStream<byte> stream, byte value)
        {
            stream.Write(value.ToSpan());
        }
        public static void WriteShort(this IStream<byte> stream, short value)
        {
            stream.Write(value.ToSpan());
        }
        public static void WriteUShort(this IStream<byte> stream, ushort value)
        {
            stream.Write(value.ToSpan());
        }
        public static void WriteInt(this IStream<byte> stream, int value)
        {
            stream.Write(value.ToSpan());
        }
        public static void WriteUInt(this IStream<byte> stream, uint value)
        {
            stream.Write(value.ToSpan());
        }
        public static void WriteLong(this IStream<byte> stream, long value)
        {
            stream.Write(value.ToSpan());
        }
        public static void WriteULong(this IStream<byte> stream, ulong value)
        {
            stream.Write(value.ToSpan());
        }
        public static void WriteFloat(this IStream<byte> stream, float value)
        {
            stream.Write(value.ToSpan());
        }
        public static void WriteDouble(this IStream<byte> stream, double value)
        {
            stream.Write(value.ToSpan());
        }
        public static void WriteDecimal(this IStream<byte> stream, decimal value)
        {
            stream.Write(value.ToSpan());
        }
        public static void WriteString(this IStream<byte> stream, string value, Encoding encoding = default)
        {
            encoding ??= Encoding.UTF8;

            stream.Write(encoding.GetBytes(value));
        }
    }
}
