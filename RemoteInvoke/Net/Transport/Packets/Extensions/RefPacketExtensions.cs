using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteInvoke.Net.Transport.Packets.Extensions
{
    public static class RefPacketExtensions
    {
        public static bool ReadBool<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Data.ReadBool();
        public static void WriteBool<T>(this ref Packet<T> packet, bool value) where T : Enum, IConvertible => packet.Data.WriteBool(value);

        public static sbyte ReadSByte<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Data.ReadSByte();
        public static void WriteSByte<T>(this ref Packet<T> packet, sbyte value) where T : Enum, IConvertible => packet.Data.WriteSByte(value);

        public static byte ReadByte<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Data.ReadByte();
        public static void WriteByte<T>(this ref Packet<T> packet, byte value) where T : Enum, IConvertible => packet.Data.WriteByte(value);

        public static short ReadShort<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Data.ReadShort();
        public static void WriteShort<T>(this ref Packet<T> packet, short value) where T : Enum, IConvertible => packet.Data.WriteShort(value);

        public static ushort ReadUShort<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Data.ReadUShort();
        public static void WriteUShort<T>(this ref Packet<T> packet, ushort value) where T : Enum, IConvertible => packet.Data.WriteUShort(value);

        public static int ReadInt<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Data.ReadInt();
        public static void WriteInt<T>(this ref Packet<T> packet, int value) where T : Enum, IConvertible => packet.Data.WriteInt(value);

        public static uint ReadUInt<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Data.ReadUInt();
        public static void WriteUInt<T>(this ref Packet<T> packet, uint value) where T : Enum, IConvertible => packet.Data.WriteUInt(value);

        public static long ReadLong<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Data.ReadLong();
        public static void WriteLong<T>(this ref Packet<T> packet, long value) where T : Enum, IConvertible => packet.Data.WriteLong(value);

        public static ulong ReadULong<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Data.ReadULong();
        public static void WriteULone<T>(this ref Packet<T> packet, ulong value) where T : Enum, IConvertible => packet.Data.WriteULong(value);

        public static float ReadFloat<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Data.ReadFloat();
        public static void WriteFloat<T>(this ref Packet<T> packet, float value) where T : Enum, IConvertible => packet.Data.WriteFloat(value);

        public static double ReadDouble<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Data.ReadDouble();
        public static void WriteDouble<T>(this ref Packet<T> packet, double value) where T : Enum, IConvertible => packet.Data.WriteDouble(value);

        public static decimal ReadDecimal<T>(this ref Packet<T> packet) where T : Enum, IConvertible => packet.Data.ReadDecimal();
        public static void WriteDecimal<T>(this ref Packet<T> packet, decimal value) where T : Enum, IConvertible => packet.Data.WriteDecimal(value);

        public static string ReadString<T>(this ref Packet<T> packet, int length, Encoding encoding) where T : Enum, IConvertible => packet.Data.ReadString(length, encoding);
        public static void WriteString<T>(this ref Packet<T> packet, string value, Encoding encoding = default) where T : Enum, IConvertible => packet.Data.WriteString(value, encoding);
    }
}
