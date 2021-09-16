using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteInvoke.Net.Transport.Abstractions;
using RemoteInvoke.Net.Transport.Packets.Extensions;
namespace RemoteInvoke.Net.Transport.Packets
{
    public class DefaultPrimitiveConverter : IPrimitivePacketConverter<PacketTypes>
    {
        public Packet<PacketTypes> Convert<T>(T value) where T : unmanaged
        {
            TypeCode code = Type.GetTypeCode(typeof(T));

            Span<byte> data = Span<byte>.Empty;

            PacketTypes type = PacketTypes.none;

            switch (code)
            {
                // organized for fastest path
                case TypeCode.UInt32:
                    if (value is uint isUint)
                    {
                        data.WriteUInt(isUint);
                    }
                    type = PacketTypes.Int;
                    break;

                case TypeCode.UInt64:
                    if (value is ulong isUlong)
                    {
                        data.WriteULong(isUlong);
                    }
                    type = PacketTypes.Long;
                    break;

                case TypeCode.Int32:
                    if (value is int isInt)
                    {
                        data.WriteInt(isInt);
                    }
                    type = PacketTypes.Int | PacketTypes.IsSignedFlag;
                    break;

                case TypeCode.Int64:
                    if (value is long isLong)
                    {
                        data.WriteLong(isLong);
                    }
                    type = PacketTypes.Long | PacketTypes.IsSignedFlag;
                    break;

                case TypeCode.Boolean:
                    if (value is bool isBool)
                    {
                        data.WriteBool(isBool);
                    }
                    type = PacketTypes.Bool;
                    break;
                case TypeCode.SByte:
                    if (value is sbyte isSbyte)
                    {
                        data.WriteSByte(isSbyte);
                    }
                    type = PacketTypes.Byte | PacketTypes.IsSignedFlag;
                    break;
                case TypeCode.Byte:
                    if (value is byte isByte)
                    {
                        data.WriteByte(isByte);
                    }
                    type = PacketTypes.Byte;
                    break;

                case TypeCode.Char:
                    if (value is char isChar)
                    {
                        data.WriteChar(isChar);
                    }
                    type = PacketTypes.Char;
                    break;
                case TypeCode.Int16:
                    if (value is short isShort)
                    {
                        data.WriteShort(isShort);
                    }
                    type = PacketTypes.Short | PacketTypes.IsSignedFlag;
                    break;
                case TypeCode.UInt16:
                    if (value is ushort isUShort)
                    {
                        data.WriteUShort(isUShort);
                    }
                    type = PacketTypes.Short;
                    break;

                case TypeCode.Single:
                    if (value is float isFloat)
                    {
                        data.WriteFloat(isFloat);
                    }
                    type = PacketTypes.Float;
                    break;
                case TypeCode.Double:
                    if (value is double isDouble)
                    {
                        data.WriteDouble(isDouble);
                    }
                    type = PacketTypes.Double;
                    break;
                case TypeCode.Decimal:
                    if (value is decimal isDecimal)
                    {
                        data.WriteDecimal(isDecimal);
                    }
                    type = PacketTypes.Decimal;
                    break;
                case TypeCode.DateTime:
                    if (value is DateTime isTime)
                    {
                        data.WriteDateTime(isTime);
                    }
                    type = PacketTypes.DateTime;
                    break;

                case TypeCode.String:
                case TypeCode.Empty:
                case TypeCode.Object:
                case TypeCode.DBNull:
                    throw new InvalidOperationException($"Failed to convert type {typeof(T).FullName} to a primitive packet");
            }

            Packet<PacketTypes> packet = new(type, data);

            return packet;
        }
    }
}
