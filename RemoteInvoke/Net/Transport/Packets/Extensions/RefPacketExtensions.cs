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
        public static bool GetBool<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Data.ReadBool();
        public static void AppendBool<T>(this ref Packet<T> packet, bool value) where T : Enum, IConvertible => packet.Data.WriteBool(value);

        public static sbyte GetSByte<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Data.ReadSByte();
        public static void AppendSByte<T>(this ref Packet<T> packet, sbyte value) where T : Enum, IConvertible => packet.Data.WriteSByte(value);

        public static byte GetByte<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Data.ReadByte();
        public static void AppendByte<T>(this ref Packet<T> packet, byte value) where T : Enum, IConvertible => packet.Data.WriteByte(value);

        public static char GetChar<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Data.ReadChar();
        public static void AppendChar<T>(this ref Packet<T> packet, char value) where T : Enum, IConvertible => packet.Data.WriteUShort(value);

        public static short GetShort<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Data.ReadShort();
        public static void AppendShort<T>(this ref Packet<T> packet, short value) where T : Enum, IConvertible => packet.Data.WriteShort(value);

        public static ushort GetUShort<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Data.ReadUShort();
        public static void AppendUShort<T>(this ref Packet<T> packet, ushort value) where T : Enum, IConvertible => packet.Data.WriteUShort(value);

        public static int GetInt<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Data.ReadInt();
        public static void AppendInt<T>(this ref Packet<T> packet, int value) where T : Enum, IConvertible => packet.Data.WriteInt(value);

        public static uint GetUInt<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Data.ReadUInt();
        public static void AppendUInt<T>(this ref Packet<T> packet, uint value) where T : Enum, IConvertible => packet.Data.WriteUInt(value);

        public static long GetLong<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Data.ReadLong();
        public static void AppendLong<T>(this ref Packet<T> packet, long value) where T : Enum, IConvertible => packet.Data.WriteLong(value);

        public static ulong GetULong<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Data.ReadULong();
        public static void AppendULong<T>(this ref Packet<T> packet, ulong value) where T : Enum, IConvertible => packet.Data.WriteULong(value);

        public static float GetFloat<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Data.ReadFloat();
        public static void AppendFloat<T>(this ref Packet<T> packet, float value) where T : Enum, IConvertible => packet.Data.WriteFloat(value);

        public static double GetDouble<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Data.ReadDouble();
        public static void AppendDouble<T>(this ref Packet<T> packet, double value) where T : Enum, IConvertible => packet.Data.WriteDouble(value);

        public static decimal GetDecimal<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Data.ReadDecimal();
        public static void AppendDecimal<T>(this ref Packet<T> packet, decimal value) where T : Enum, IConvertible => packet.Data.WriteDecimal(value);

        public static string GetString<T>(this ref Packet<T> packet, int length, Encoding encoding) where T : Enum, IConvertible => packet.Data.ReadString(length, encoding);
        public static void AppendString<T>(this ref Packet<T> packet, string value, Encoding encoding = default) where T : Enum, IConvertible => packet.Data.WriteString(value, encoding);

        public static DateTime GetDateTime<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Data.ReadDateTime();
        public static void AppendDateTime<T>(this ref Packet<T> packet, DateTime value) where T : Enum, IConvertible => packet.Data.WriteDateTime(value);

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
