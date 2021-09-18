using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetInterop.Transport.Core.Delegates;

namespace NetInterop.Transport.Core.Packets.Extensions
{
    public static class RefPacketExtensions
    {
        public static bool GetBool<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Remove(sizeof(bool)).ToBool();
        public static void AppendBool<T>(this ref Packet<T> packet, bool value) where T : Enum, IConvertible => packet.Append(value.ToSpan());

        public static sbyte GetSByte<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Remove(1).ToSByte();
        public static void AppendSByte<T>(this ref Packet<T> packet, sbyte value) where T : Enum, IConvertible => packet.Append(value.ToSpan());

        public static byte GetByte<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Remove(1).ToByte();
        public static void AppendByte<T>(this ref Packet<T> packet, byte value) where T : Enum, IConvertible => packet.Append(value.ToSpan());

        public static char GetChar<T>(this ref Packet<T> packet) where T : Enum, IConvertible => (char)packet.Remove(sizeof(char)).ToUShort();
        public static void AppendChar<T>(this ref Packet<T> packet, char value) where T : Enum, IConvertible => packet.Append(((ushort)value).ToSpan());

        public static short GetShort<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Remove(sizeof(short)).ToShort();
        public static void AppendShort<T>(this ref Packet<T> packet, short value) where T : Enum, IConvertible => packet.Append(value.ToSpan());

        public static ushort GetUShort<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Remove(sizeof(ushort)).ToUShort();
        public static void AppendUShort<T>(this ref Packet<T> packet, ushort value) where T : Enum, IConvertible => packet.Append(value.ToSpan());

        public static int GetInt<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Remove(sizeof(int)).ToInt();
        public static void AppendInt<T>(this ref Packet<T> packet, int value) where T : Enum, IConvertible => packet.Append(value.ToSpan());

        public static uint GetUInt<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Remove(sizeof(uint)).ToUInt();
        public static void AppendUInt<T>(this ref Packet<T> packet, uint value) where T : Enum, IConvertible => packet.Append(value.ToSpan());

        public static long GetLong<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Remove(sizeof(long)).ToLong();
        public static void AppendLong<T>(this ref Packet<T> packet, long value) where T : Enum, IConvertible => packet.Append(value.ToSpan());

        public static ulong GetULong<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Remove(sizeof(ulong)).ToULong();
        public static void AppendULong<T>(this ref Packet<T> packet, ulong value) where T : Enum, IConvertible => packet.Append(value.ToSpan());

        public static float GetFloat<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Remove(sizeof(float)).ToFloat();
        public static void AppendFloat<T>(this ref Packet<T> packet, float value) where T : Enum, IConvertible => packet.Append(value.ToSpan());

        public static double GetDouble<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Remove(sizeof(double)).ToDouble();
        public static void AppendDouble<T>(this ref Packet<T> packet, double value) where T : Enum, IConvertible => packet.Append(value.ToSpan());

        public static decimal GetDecimal<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Remove(sizeof(decimal)).ToDecimal();
        public static void AppendDecimal<T>(this ref Packet<T> packet, decimal value) where T : Enum, IConvertible => packet.Append(value.ToSpan());

        public static string GetString<T>(this ref Packet<T> packet, Encoding encoding) where T : Enum, IConvertible
        {
            int length = packet.Remove(4).ToInt();
            return packet.Remove(length).ToString(encoding);
        }
        public static void AppendString<T>(this ref Packet<T> packet, string value, Encoding encoding = default) where T : Enum, IConvertible
        {
            packet.Append(value.Length.ToSpan());
            packet.Append(value.ToSpan(encoding));
        }

        public static DateTime GetDateTime<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Remove(sizeof(long)).ToDateTime();
        public static void AppendDateTime<T>(this ref Packet<T> packet, DateTime value) where T : Enum, IConvertible => packet.Append(value.ToSpan());
        public static void AppendArray<TPacket>(this ref Packet<TPacket> packet, sbyte[] array) where TPacket : Enum, IConvertible
        {
            AppendArray(ref packet, array, SpanTypeConversionExtensions.ToSpan);
        }
        public static void AppendArray<TPacket>(this ref Packet<TPacket> packet, byte[] array) where TPacket : Enum, IConvertible
        {
            AppendArray(ref packet, array, SpanTypeConversionExtensions.ToSpan);
        }
        public static void AppendArray<TPacket>(this ref Packet<TPacket> packet, ushort[] array) where TPacket : Enum, IConvertible
        {
            AppendArray(ref packet, array, SpanTypeConversionExtensions.ToSpan);
        }
        public static void AppendArray<TPacket>(this ref Packet<TPacket> packet, short[] array) where TPacket : Enum, IConvertible
        {
            AppendArray(ref packet, array, SpanTypeConversionExtensions.ToSpan);
        }
        public static void AppendArray<TPacket>(this ref Packet<TPacket> packet, int[] array) where TPacket : Enum, IConvertible
        {
            AppendArray(ref packet, array, SpanTypeConversionExtensions.ToSpan);
        }
        public static void AppendArray<TPacket>(this ref Packet<TPacket> packet, uint[] array) where TPacket : Enum, IConvertible
        {
            AppendArray(ref packet, array, SpanTypeConversionExtensions.ToSpan);
        }
        public static void AppendArray<TPacket>(this ref Packet<TPacket> packet, long[] array) where TPacket : Enum, IConvertible
        {
            AppendArray(ref packet, array, SpanTypeConversionExtensions.ToSpan);
        }
        public static void AppendArray<TPacket>(this ref Packet<TPacket> packet, ulong[] array) where TPacket : Enum, IConvertible
        {
            AppendArray(ref packet, array, SpanTypeConversionExtensions.ToSpan);
        }
        public static void AppendArray<TPacket>(this ref Packet<TPacket> packet, float[] array) where TPacket : Enum, IConvertible
        {
            AppendArray(ref packet, array, SpanTypeConversionExtensions.ToSpan);
        }
        public static void AppendArray<TPacket>(this ref Packet<TPacket> packet, double[] array) where TPacket : Enum, IConvertible
        {
            AppendArray(ref packet, array, SpanTypeConversionExtensions.ToSpan);
        }
        public static void AppendArray<TPacket>(this ref Packet<TPacket> packet, decimal[] array) where TPacket : Enum, IConvertible
        {
            AppendArray(ref packet, array, SpanTypeConversionExtensions.ToSpan);
        }
        public static void AppendArray<TPacket>(this ref Packet<TPacket> packet, DateTime[] array) where TPacket : Enum, IConvertible
        {
            AppendArray(ref packet, array, SpanTypeConversionExtensions.ToSpan);
        }

        public static bool[] GetBoolArray<TPacket>(this ref Packet<TPacket> packet) where TPacket : Enum, IConvertible
        {
            return GetArray(ref packet, SpanTypeConversionExtensions.ToBool);
        }
        public static sbyte[] GetArray<TPacket>(this ref Packet<TPacket> packet) where TPacket : Enum, IConvertible
        {
            return GetArray(ref packet, SpanTypeConversionExtensions.ToSByte);
        }
        public static byte[] GetByteArray<TPacket>(this ref Packet<TPacket> packet) where TPacket : Enum, IConvertible
        {
            return GetArray(ref packet, SpanTypeConversionExtensions.ToByte);
        }
        public static short[] GetShortArray<TPacket>(this ref Packet<TPacket> packet) where TPacket : Enum, IConvertible
        {
            return GetArray(ref packet, SpanTypeConversionExtensions.ToShort);
        }
        public static ushort[] GetUShortArray<TPacket>(this ref Packet<TPacket> packet) where TPacket : Enum, IConvertible
        {
            return GetArray(ref packet, SpanTypeConversionExtensions.ToUShort);
        }
        public static int[] GetIntArray<TPacket>(this ref Packet<TPacket> packet) where TPacket : Enum, IConvertible
        {
            return GetArray(ref packet, SpanTypeConversionExtensions.ToInt);
        }
        public static uint[] GetUIntArray<TPacket>(this ref Packet<TPacket> packet) where TPacket : Enum, IConvertible
        {
            return GetArray(ref packet, SpanTypeConversionExtensions.ToUInt);
        }
        public static long[] GetLongArray<TPacket>(this ref Packet<TPacket> packet) where TPacket : Enum, IConvertible
        {
            return GetArray(ref packet, SpanTypeConversionExtensions.ToLong);
        }
        public static ulong[] GetULongArray<TPacket>(this ref Packet<TPacket> packet) where TPacket : Enum, IConvertible
        {
            return GetArray(ref packet, SpanTypeConversionExtensions.ToULong);
        }
        public static float[] GetFloatArray<TPacket>(this ref Packet<TPacket> packet) where TPacket : Enum, IConvertible
        {
            return GetArray(ref packet, SpanTypeConversionExtensions.ToFloat);
        }
        public static double[] GetDoubleArray<TPacket>(this ref Packet<TPacket> packet) where TPacket : Enum, IConvertible
        {
            return GetArray(ref packet, SpanTypeConversionExtensions.ToDouble);
        }
        public static decimal[] GetDecimalArray<TPacket>(this ref Packet<TPacket> packet) where TPacket : Enum, IConvertible
        {
            return GetArray(ref packet, SpanTypeConversionExtensions.ToDecimal);
        }
        public static DateTime[] GetDateTimeArray<TPacket>(this ref Packet<TPacket> packet) where TPacket : Enum, IConvertible
        {
            return GetArray(ref packet, SpanTypeConversionExtensions.ToDateTime);
        }

        public unsafe static void AppendArray<TPacket, T>(this ref Packet<TPacket> packet, T[] array, ToSpanFunc<T, byte> objectToSpanConverter) where TPacket : Enum, IConvertible where T : unmanaged
        {
            // arrays should always 
            packet.AppendInt(array.Length);

            int size = sizeof(T);

            // more efficient to get a portion of the buffer manually instead of calling append over and over again
            array.ToSpan(packet.GetBuffer(array.Length * size), size, objectToSpanConverter);
        }

        public unsafe static T[] GetArray<TPacket, T>(this ref Packet<TPacket> packet, FromSpanFunc<byte, T> spanToObjectConverter) where TPacket : Enum, IConvertible where T : unmanaged
        {
            int length = packet.GetInt();

            return packet.Remove(length * sizeof(T)).ToArray<T>(sizeof(T), spanToObjectConverter);
        }
    }
}
