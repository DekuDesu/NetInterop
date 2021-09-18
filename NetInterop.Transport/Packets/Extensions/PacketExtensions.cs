using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Delegates;
using System;
using System.Text;

namespace NetInterop.Transport.Core.Packets.Extensions
{
    public static class PacketExtensions
    {
        public static bool GetBool<T>(this IPacket<T> packet) where T : Enum, IConvertible => packet.Remove(sizeof(bool)).ToBool();
        public static void AppendBool<T>(this IPacket<T> packet, bool value) where T : Enum, IConvertible => packet.GetBuffer(sizeof(bool)).Write(value);

        public static sbyte GetSByte<T>(this IPacket<T> packet) where T : Enum, IConvertible => (sbyte)packet.Remove(1);
        public static void AppendSByte<T>(this IPacket<T> packet, sbyte value) where T : Enum, IConvertible => packet.GetBuffer(1) = (byte)value;

        public static byte GetByte<T>(this IPacket<T> packet) where T : Enum, IConvertible => packet.Remove(1).ToByte();
        public static void AppendByte<T>(this IPacket<T> packet, byte value) where T : Enum, IConvertible => packet.GetBuffer(sizeof(byte)).Write(value);

        public static char GetChar<T>(this IPacket<T> packet) where T : Enum, IConvertible => (char)packet.Remove(sizeof(char)).ToUShort();
        public static void AppendChar<T>(this IPacket<T> packet, char value) where T : Enum, IConvertible => packet.GetBuffer(sizeof(char)).Write(value);

        public static short GetShort<T>(this IPacket<T> packet) where T : Enum, IConvertible => packet.Remove(sizeof(short)).ToShort();
        public static void AppendShort<T>(this IPacket<T> packet, short value) where T : Enum, IConvertible => packet.GetBuffer(sizeof(short)).Write(value);

        public static ushort GetUShort<T>(this IPacket<T> packet) where T : Enum, IConvertible => packet.Remove(sizeof(ushort)).ToUShort();
        public static void AppendUShort<T>(this IPacket<T> packet, ushort value) where T : Enum, IConvertible => packet.GetBuffer(sizeof(ushort)).Write(value);

        public static int GetInt<T>(this IPacket<T> packet) where T : Enum, IConvertible => packet.Remove(sizeof(int)).ToInt();
        public static void AppendInt<T>(this IPacket<T> packet, int value) where T : Enum, IConvertible => packet.GetBuffer(sizeof(int)).Write(value);

        public static uint GetUInt<T>(this IPacket<T> packet) where T : Enum, IConvertible => packet.Remove(sizeof(uint)).ToUInt();
        public static void AppendUInt<T>(this IPacket<T> packet, uint value) where T : Enum, IConvertible => packet.GetBuffer(sizeof(uint)).Write(value);

        public static long GetLong<T>(this IPacket<T> packet) where T : Enum, IConvertible => packet.Remove(sizeof(long)).ToLong();
        public static void AppendLong<T>(this IPacket<T> packet, long value) where T : Enum, IConvertible => packet.GetBuffer(sizeof(long)).Write(value);

        public static ulong GetULong<T>(this IPacket<T> packet) where T : Enum, IConvertible => packet.Remove(sizeof(ulong)).ToULong();
        public static void AppendULong<T>(this IPacket<T> packet, ulong value) where T : Enum, IConvertible => packet.GetBuffer(sizeof(ulong)).Write(value);

        public static float GetFloat<T>(this IPacket<T> packet) where T : Enum, IConvertible => packet.Remove(sizeof(float)).ToFloat();
        public static void AppendFloat<T>(this IPacket<T> packet, float value) where T : Enum, IConvertible => packet.GetBuffer(sizeof(float)).Write(value);

        public static double GetDouble<T>(this IPacket<T> packet) where T : Enum, IConvertible => packet.Remove(sizeof(double)).ToDouble();
        public static void AppendDouble<T>(this IPacket<T> packet, double value) where T : Enum, IConvertible => packet.GetBuffer(sizeof(double)).Write(value);

        public static decimal GetDecimal<T>(this IPacket<T> packet) where T : Enum, IConvertible => packet.Remove(sizeof(decimal)).ToDecimal();
        public static void AppendDecimal<T>(this IPacket<T> packet, decimal value) where T : Enum, IConvertible => packet.GetBuffer(sizeof(decimal)).Write(value);

        public static string GetString<T>(this IPacket<T> packet, Encoding encoding) where T : Enum, IConvertible
        {
            int length = packet.Remove(4).ToInt();
            return packet.Remove(length).ToString(length, encoding);
        }
        public static void AppendString<T>(this IPacket<T> packet, string value, Encoding encoding = default) where T : Enum, IConvertible
        {
            packet.GetBuffer(sizeof(int)).Write(value.Length);
            packet.Append(encoding.GetBytes(value));
        }

        public static DateTime GetDateTime<T>(this IPacket<T> packet) where T : Enum, IConvertible => packet.Remove(sizeof(long)).ToDateTime();
        public static void AppendDateTime<T>(this IPacket<T> packet, DateTime value) where T : Enum, IConvertible => packet.GetBuffer(sizeof(long)).Write(value);

        public static void AppendSerializable<T>(this IPacket<T> packet, IPacketSerializable<T> serializableValue) where T : Enum
        {
            if (serializableValue is null)
            {
                throw new NullReferenceException(nameof(serializableValue));
            }
            serializableValue.Serialize(packet);
        }

        public static T GetDeserializable<TPacket, T>(this IPacket<TPacket> packet, IPacketDeserializable<TPacket, T> deserializableReference) where TPacket : Enum
        {
            return deserializableReference.Deserialize(packet);
        }

        public static void AppendArray<TPacket>(this IPacket<TPacket> packet, sbyte[] array) where TPacket : Enum, IConvertible
        {
            AppendArray(packet, array, sizeof(sbyte), PointerExtensions.Write);
        }
        public static void AppendArray<TPacket>(this IPacket<TPacket> packet, byte[] array) where TPacket : Enum, IConvertible
        {
            AppendArray(packet, array, sizeof(byte), PointerExtensions.Write);
        }
        public static void AppendArray<TPacket>(this IPacket<TPacket> packet, ushort[] array) where TPacket : Enum, IConvertible
        {
            AppendArray(packet, array, sizeof(ushort), PointerExtensions.Write);
        }
        public static void AppendArray<TPacket>(this IPacket<TPacket> packet, short[] array) where TPacket : Enum, IConvertible
        {
            AppendArray(packet, array, sizeof(short), PointerExtensions.Write);
        }
        public static void AppendArray<TPacket>(this IPacket<TPacket> packet, int[] array) where TPacket : Enum, IConvertible
        {
            AppendArray(packet, array, sizeof(int), PointerExtensions.Write);
        }
        public static void AppendArray<TPacket>(this IPacket<TPacket> packet, uint[] array) where TPacket : Enum, IConvertible
        {
            AppendArray(packet, array, sizeof(uint), PointerExtensions.Write);
        }
        public static void AppendArray<TPacket>(this IPacket<TPacket> packet, long[] array) where TPacket : Enum, IConvertible
        {
            AppendArray(packet, array, sizeof(long), PointerExtensions.Write);
        }
        public static void AppendArray<TPacket>(this IPacket<TPacket> packet, ulong[] array) where TPacket : Enum, IConvertible
        {
            AppendArray(packet, array, sizeof(ulong), PointerExtensions.Write);
        }
        public static void AppendArray<TPacket>(this IPacket<TPacket> packet, float[] array) where TPacket : Enum, IConvertible
        {
            AppendArray(packet, array, sizeof(float), PointerExtensions.Write);
        }
        public static void AppendArray<TPacket>(this IPacket<TPacket> packet, double[] array) where TPacket : Enum, IConvertible
        {
            AppendArray(packet, array, sizeof(double), PointerExtensions.Write);
        }
        public static void AppendArray<TPacket>(this IPacket<TPacket> packet, decimal[] array) where TPacket : Enum, IConvertible
        {
            AppendArray(packet, array, sizeof(decimal), PointerExtensions.Write);
        }
        public static void AppendArray<TPacket>(this IPacket<TPacket> packet, DateTime[] array) where TPacket : Enum, IConvertible
        {
            AppendArray(packet, array, sizeof(long), PointerExtensions.Write);
        }

        public static bool[] GetBoolArray<TPacket>(this IPacket<TPacket> packet) where TPacket : Enum, IConvertible
        {
            return GetArray(packet, sizeof(bool), PointerExtensions.ToBool);
        }
        public static sbyte[] GetArray<TPacket>(this IPacket<TPacket> packet) where TPacket : Enum, IConvertible
        {
            return GetArray(packet, sizeof(sbyte), PointerExtensions.ToSByte);
        }
        public static byte[] GetByteArray<TPacket>(this IPacket<TPacket> packet) where TPacket : Enum, IConvertible
        {
            return GetArray(packet, sizeof(byte), PointerExtensions.ToByte);
        }
        public static short[] GetShortArray<TPacket>(this IPacket<TPacket> packet) where TPacket : Enum, IConvertible
        {
            return GetArray(packet, sizeof(short), PointerExtensions.ToShort);
        }
        public static ushort[] GetUShortArray<TPacket>(this IPacket<TPacket> packet) where TPacket : Enum, IConvertible
        {
            return GetArray(packet, sizeof(ushort), PointerExtensions.ToUShort);
        }
        public static int[] GetIntArray<TPacket>(this IPacket<TPacket> packet) where TPacket : Enum, IConvertible
        {
            return GetArray(packet, sizeof(int), PointerExtensions.ToInt);
        }
        public static uint[] GetUIntArray<TPacket>(this IPacket<TPacket> packet) where TPacket : Enum, IConvertible
        {
            return GetArray(packet, sizeof(uint), PointerExtensions.ToUInt);
        }
        public static long[] GetLongArray<TPacket>(this IPacket<TPacket> packet) where TPacket : Enum, IConvertible
        {
            return GetArray(packet, sizeof(long), PointerExtensions.ToLong);
        }
        public static ulong[] GetULongArray<TPacket>(this IPacket<TPacket> packet) where TPacket : Enum, IConvertible
        {
            return GetArray(packet, sizeof(ulong), PointerExtensions.ToULong);
        }
        public static float[] GetFloatArray<TPacket>(this IPacket<TPacket> packet) where TPacket : Enum, IConvertible
        {
            return GetArray(packet, sizeof(float), PointerExtensions.ToFloat);
        }
        public static double[] GetDoubleArray<TPacket>(this IPacket<TPacket> packet) where TPacket : Enum, IConvertible
        {
            return GetArray(packet, sizeof(double), PointerExtensions.ToDouble);
        }
        public static decimal[] GetDecimalArray<TPacket>(this IPacket<TPacket> packet) where TPacket : Enum, IConvertible
        {
            return GetArray(packet, sizeof(decimal), PointerExtensions.ToDecimal);
        }
        public static DateTime[] GetDateTimeArray<TPacket>(this IPacket<TPacket> packet) where TPacket : Enum, IConvertible
        {
            return GetArray(packet, sizeof(long), PointerExtensions.ToDateTime);
        }

        public static void AppendArray<TPacket, T>(this IPacket<TPacket> packet, T[] array, int sizeOfT, PointerAction<T> objectAppender) where TPacket : Enum, IConvertible where T : unmanaged
        {
            // arrays should always 
            packet.AppendInt(array.Length);

            for (int i = 0; i < array.Length; i++)
            {
                ref byte ptr = ref packet.GetBuffer(sizeOfT);

                objectAppender(ref ptr, array[i]);
            }
        }

        public static T[] GetArray<TPacket, T>(this IPacket<TPacket> packet, int sizeOfT, PointerFunc<T> ptrToObjectConverter) where TPacket : Enum, IConvertible where T : unmanaged
        {
            // get how many elements should be present
            int length = packet.GetInt();

            T[] result = new T[length];

            for (int i = 0; i < length; i++)
            {
                ref byte ptr = ref packet.Remove(sizeOfT);

                result[i] = ptrToObjectConverter(ref ptr);
            }

            return result;
        }
    }
}
