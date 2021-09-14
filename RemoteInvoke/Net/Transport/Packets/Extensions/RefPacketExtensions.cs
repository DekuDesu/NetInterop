using RemoteInvoke.Net.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace RemoteInvoke.Net.Transport.Packets.Extensions
{
    public static class RefPacketExtensions
    {
        public static bool GetBool<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.RemoveStart(sizeof(bool)).ToBool();
        public static void AppendBool<T>(this ref Packet<T> packet, bool value) where T : Enum, IConvertible => packet.Append(value.ToSpan());

        public static sbyte GetSByte<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.RemoveStart(1).ToSByte();
        public static void AppendSByte<T>(this ref Packet<T> packet, sbyte value) where T : Enum, IConvertible => packet.Append(value.ToSpan());

        public static byte GetByte<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.RemoveStart(1).ToByte();
        public static void AppendByte<T>(this ref Packet<T> packet, byte value) where T : Enum, IConvertible => packet.Append(value.ToSpan());

        public static char GetChar<T>(this ref Packet<T> packet) where T : Enum, IConvertible => (char)packet.RemoveStart(sizeof(char)).ToUShort();
        public static void AppendChar<T>(this ref Packet<T> packet, char value) where T : Enum, IConvertible => packet.Append(((ushort)value).ToSpan());

        public static short GetShort<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.RemoveStart(sizeof(short)).ToShort();
        public static void AppendShort<T>(this ref Packet<T> packet, short value) where T : Enum, IConvertible => packet.Append(value.ToSpan());

        public static ushort GetUShort<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.RemoveStart(sizeof(ushort)).ToUShort();
        public static void AppendUShort<T>(this ref Packet<T> packet, ushort value) where T : Enum, IConvertible => packet.Append(value.ToSpan());

        public static int GetInt<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.RemoveStart(sizeof(int)).ToInt();
        public static void AppendInt<T>(this ref Packet<T> packet, int value) where T : Enum, IConvertible => packet.Append(value.ToSpan());

        public static uint GetUInt<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.RemoveStart(sizeof(uint)).ToUInt();
        public static void AppendUInt<T>(this ref Packet<T> packet, uint value) where T : Enum, IConvertible => packet.Append(value.ToSpan());

        public static long GetLong<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.RemoveStart(sizeof(long)).ToLong();
        public static void AppendLong<T>(this ref Packet<T> packet, long value) where T : Enum, IConvertible => packet.Append(value.ToSpan());

        public static ulong GetULong<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.RemoveStart(sizeof(ulong)).ToULong();
        public static void AppendULong<T>(this ref Packet<T> packet, ulong value) where T : Enum, IConvertible => packet.Append(value.ToSpan());

        public static float GetFloat<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.RemoveStart(sizeof(float)).ToFloat();
        public static void AppendFloat<T>(this ref Packet<T> packet, float value) where T : Enum, IConvertible => packet.Append(value.ToSpan());

        public static double GetDouble<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.RemoveStart(sizeof(double)).ToDouble();
        public static void AppendDouble<T>(this ref Packet<T> packet, double value) where T : Enum, IConvertible => packet.Append(value.ToSpan());

        public static decimal GetDecimal<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.RemoveStart(sizeof(decimal)).ToDecimal();
        public static void AppendDecimal<T>(this ref Packet<T> packet, decimal value) where T : Enum, IConvertible => packet.Append(value.ToSpan());

        public static string GetString<T>(this ref Packet<T> packet, int length, Encoding encoding) where T : Enum, IConvertible => packet.RemoveStart(length).ToString(encoding);
        public static void AppendString<T>(this ref Packet<T> packet, string value, Encoding encoding = default) where T : Enum, IConvertible => packet.Append(value.ToSpan(encoding));

        public static DateTime GetDateTime<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.RemoveStart(sizeof(long)).ToDateTime();
        public static void AppendDateTime<T>(this ref Packet<T> packet, DateTime value) where T : Enum, IConvertible => packet.Append(value.ToSpan());

        public static void AppendSerializable<T>(this ref Packet<T> packet, IPacketSerializable<T> serializableValue) where T : Enum
        {
            if (serializableValue is null)
            {
                throw new NullReferenceException(nameof(serializableValue));
            }
            serializableValue.Serialize(ref packet);
        }

        public static T GetDeserializable<TPacket, T>(this ref Packet<TPacket> packet, IPacketDeserializable<TPacket, T> deserializableReference) where TPacket : Enum
        {
            return deserializableReference.Deserialize(packet);
        }
    }
}
