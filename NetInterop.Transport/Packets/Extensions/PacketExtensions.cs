using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Delegates;
using System;
using System.Diagnostics;
using System.Text;

namespace NetInterop.Transport.Core.Packets.Extensions
{
    public static class PacketExtensions
    {
        [DebuggerHidden]
        public static bool GetBool(this IPacket packet) => packet.Remove(sizeof(bool)).ToBool();
        [DebuggerHidden]
        public static void AppendBool(this IPacket packet, bool value) => packet.GetBuffer(sizeof(bool)).Write(value);

        [DebuggerHidden]
        public static sbyte GetSByte(this IPacket packet) => (sbyte)packet.Remove(1);
        [DebuggerHidden]
        public static void AppendSByte(this IPacket packet, sbyte value) => packet.GetBuffer(1) = (byte)value;

        [DebuggerHidden]
        public static byte GetByte(this IPacket packet) => packet.Remove(1).ToByte();
        [DebuggerHidden]
        public static void AppendByte(this IPacket packet, byte value) => packet.GetBuffer(sizeof(byte)).Write(value);

        [DebuggerHidden]
        public static char GetChar(this IPacket packet) => (char)packet.Remove(sizeof(char)).ToUShort();
        [DebuggerHidden]
        public static void AppendChar(this IPacket packet, char value) => packet.GetBuffer(sizeof(char)).Write(value);

        [DebuggerHidden]
        public static short GetShort(this IPacket packet) => packet.Remove(sizeof(short)).ToShort();
        [DebuggerHidden]
        public static void AppendShort(this IPacket packet, short value) => packet.GetBuffer(sizeof(short)).Write(value);

        [DebuggerHidden]
        public static ushort GetUShort(this IPacket packet) => packet.Remove(sizeof(ushort)).ToUShort();
        [DebuggerHidden]
        public static void AppendUShort(this IPacket packet, ushort value) => packet.GetBuffer(sizeof(ushort)).Write(value);

        [DebuggerHidden]
        public static int GetInt(this IPacket packet) => packet.Remove(sizeof(int)).ToInt();
        [DebuggerHidden]
        public static void AppendInt(this IPacket packet, int value) => packet.GetBuffer(sizeof(int)).Write(value);

        [DebuggerHidden]
        public static uint GetUInt(this IPacket packet) => packet.Remove(sizeof(uint)).ToUInt();
        [DebuggerHidden]
        public static void AppendUInt(this IPacket packet, uint value) => packet.GetBuffer(sizeof(uint)).Write(value);

        [DebuggerHidden]
        public static long GetLong(this IPacket packet) => packet.Remove(sizeof(long)).ToLong();
        [DebuggerHidden]
        public static void AppendLong(this IPacket packet, long value) => packet.GetBuffer(sizeof(long)).Write(value);

        [DebuggerHidden]
        public static ulong GetULong(this IPacket packet) => packet.Remove(sizeof(ulong)).ToULong();
        [DebuggerHidden]
        public static void AppendULong(this IPacket packet, ulong value) => packet.GetBuffer(sizeof(ulong)).Write(value);

        [DebuggerHidden]
        public static float GetFloat(this IPacket packet) => packet.Remove(sizeof(float)).ToFloat();
        [DebuggerHidden]
        public static void AppendFloat(this IPacket packet, float value) => packet.GetBuffer(sizeof(float)).Write(value);

        [DebuggerHidden]
        public static double GetDouble(this IPacket packet) => packet.Remove(sizeof(double)).ToDouble();
        [DebuggerHidden]
        public static void AppendDouble(this IPacket packet, double value) => packet.GetBuffer(sizeof(double)).Write(value);

        [DebuggerHidden]
        public static decimal GetDecimal(this IPacket packet) => packet.Remove(sizeof(decimal)).ToDecimal();
        [DebuggerHidden]
        public static void AppendDecimal(this IPacket packet, decimal value) => packet.GetBuffer(sizeof(decimal)).Write(value);

        [DebuggerHidden]
        public static string GetString(this IPacket packet, Encoding encoding)
        {
            int length = packet.Remove(4).ToInt();
            return packet.Remove(length).ToString(length, encoding);
        }
        [DebuggerHidden]
        public static void AppendString(this IPacket packet, string value, Encoding encoding = default)
        {
            packet.GetBuffer(sizeof(int)).Write(value.Length);
            packet.Append(encoding.GetBytes(value));
        }

        [DebuggerHidden]
        public static DateTime GetDateTime(this IPacket packet) => packet.Remove(sizeof(long)).ToDateTime();

        [DebuggerHidden]
        public static void AppendDateTime(this IPacket packet, DateTime value) => packet.GetBuffer(sizeof(long)).Write(value);

        [DebuggerHidden]
        public static void AppendSerializable(this IPacket packet, IPacketSerializable serializableValue)
        {
            if (serializableValue is null)
            {
                throw new NullReferenceException(nameof(serializableValue));
            }
            serializableValue.Serialize(packet);
        }

        [DebuggerHidden]
        public static T GetDeserializable<T>(this IPacket packet, IPacketDeserializable<T> deserializableReference)
        {
            return deserializableReference.Deserialize(packet);
        }
        [DebuggerHidden]
        public static void AppendArray(this IPacket packet, sbyte[] array)
        {
            AppendArray(packet, array, sizeof(sbyte), PointerExtensions.Write);
        }
        [DebuggerHidden]
        public static void AppendArray(this IPacket packet, byte[] array)
        {
            AppendArray(packet, array, sizeof(byte), PointerExtensions.Write);
        }
        [DebuggerHidden]
        public static void AppendArray(this IPacket packet, ushort[] array)
        {
            AppendArray(packet, array, sizeof(ushort), PointerExtensions.Write);
        }
        [DebuggerHidden]
        public static void AppendArray(this IPacket packet, short[] array)
        {
            AppendArray(packet, array, sizeof(short), PointerExtensions.Write);
        }
        [DebuggerHidden]
        public static void AppendArray(this IPacket packet, int[] array)
        {
            AppendArray(packet, array, sizeof(int), PointerExtensions.Write);
        }
        [DebuggerHidden]
        public static void AppendArray(this IPacket packet, uint[] array)
        {
            AppendArray(packet, array, sizeof(uint), PointerExtensions.Write);
        }
        [DebuggerHidden]
        public static void AppendArray(this IPacket packet, long[] array)
        {
            AppendArray(packet, array, sizeof(long), PointerExtensions.Write);
        }
        [DebuggerHidden]
        public static void AppendArray(this IPacket packet, ulong[] array)
        {
            AppendArray(packet, array, sizeof(ulong), PointerExtensions.Write);
        }
        [DebuggerHidden]
        public static void AppendArray(this IPacket packet, float[] array)
        {
            AppendArray(packet, array, sizeof(float), PointerExtensions.Write);
        }
        [DebuggerHidden]
        public static void AppendArray(this IPacket packet, double[] array)
        {
            AppendArray(packet, array, sizeof(double), PointerExtensions.Write);
        }
        [DebuggerHidden]
        public static void AppendArray(this IPacket packet, decimal[] array)
        {
            AppendArray(packet, array, sizeof(decimal), PointerExtensions.Write);
        }
        [DebuggerHidden]
        public static void AppendArray(this IPacket packet, DateTime[] array)
        {
            AppendArray(packet, array, sizeof(long), PointerExtensions.Write);
        }
        [DebuggerHidden]
        public static bool[] GetBoolArray(this IPacket packet)
        {
            return GetArray(packet, sizeof(bool), PointerExtensions.ToBool);
        }
        [DebuggerHidden]
        public static sbyte[] GetArray(this IPacket packet)
        {
            return GetArray(packet, sizeof(sbyte), PointerExtensions.ToSByte);
        }
        [DebuggerHidden]
        public static byte[] GetByteArray(this IPacket packet)
        {
            return GetArray(packet, sizeof(byte), PointerExtensions.ToByte);
        }
        [DebuggerHidden]
        public static short[] GetShortArray(this IPacket packet)
        {
            return GetArray(packet, sizeof(short), PointerExtensions.ToShort);
        }
        [DebuggerHidden]
        public static ushort[] GetUShortArray(this IPacket packet)
        {
            return GetArray(packet, sizeof(ushort), PointerExtensions.ToUShort);
        }
        [DebuggerHidden]
        public static int[] GetIntArray(this IPacket packet)
        {
            return GetArray(packet, sizeof(int), PointerExtensions.ToInt);
        }
        [DebuggerHidden]
        public static uint[] GetUIntArray(this IPacket packet)
        {
            return GetArray(packet, sizeof(uint), PointerExtensions.ToUInt);
        }
        [DebuggerHidden]
        public static long[] GetLongArray(this IPacket packet)
        {
            return GetArray(packet, sizeof(long), PointerExtensions.ToLong);
        }
        [DebuggerHidden]
        public static ulong[] GetULongArray(this IPacket packet)
        {
            return GetArray(packet, sizeof(ulong), PointerExtensions.ToULong);
        }

        [DebuggerHidden]
        public static float[] GetFloatArray(this IPacket packet)
        {
            return GetArray(packet, sizeof(float), PointerExtensions.ToFloat);
        }
        [DebuggerHidden]
        public static double[] GetDoubleArray(this IPacket packet)
        {
            return GetArray(packet, sizeof(double), PointerExtensions.ToDouble);
        }
        [DebuggerHidden]
        public static decimal[] GetDecimalArray(this IPacket packet)
        {
            return GetArray(packet, sizeof(decimal), PointerExtensions.ToDecimal);
        }
        [DebuggerHidden]
        public static DateTime[] GetDateTimeArray(this IPacket packet)
        {
            return GetArray(packet, sizeof(long), PointerExtensions.ToDateTime);
        }

        [DebuggerHidden]
        public static void AppendArray<T>(this IPacket packet, T[] array, int sizeOfT, PointerAction<T> objectAppender) where T : unmanaged
        {
            // arrays should always 
            packet.AppendInt(array.Length);

            for (int i = 0; i < array.Length; i++)
            {
                ref byte ptr = ref packet.GetBuffer(sizeOfT);

                objectAppender(ref ptr, array[i]);
            }
        }

        [DebuggerHidden]
        public static T[] GetArray<T>(this IPacket packet, int sizeOfT, PointerFunc<T> ptrToObjectConverter) where T : unmanaged
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
