using NetInterop.Transport.Core.Abstractions;
using NetInterop.Transport.Core.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Transport.Core
{
    public class DefaultHeader<TPacket> : IPacketHeader<TPacket> where TPacket : Enum, IConvertible
    {
        private readonly Func<int, TPacket> converter;

        public DefaultHeader(Func<int, TPacket> converter = null)
        {
            this.converter = converter ?? ((num) => (TPacket)Enum.ToObject(typeof(TPacket), num));
        }

        /*
            HEADER SPECIFICATION
            header is stored in uint
            header should be stored in the first 4 bytes of a packet
            header composition is as follows
            
            First 2 bytes: message size(the length of the packet that follows the header)
            Second 2 bytes: message type (the ushort id of the message used as a unique identifier)****

            ****This is implementation defined, header specification does not require any information or care about any information within these bytes
        */

        public void CreateHeader(ref Packet<TPacket> packet)
        {
            Span<byte> header = packet.GetHeaderBytes();

            packet.PacketType.ToUInt16(null).ToSpan().CopyTo(header.Slice(2, 2));

            BitConverter.GetBytes((ushort)packet.Length).CopyTo(header.Slice(0, 2));
        }

        public TPacket GetHeaderType(Span<byte> headerBytes)
        {
            return converter(headerBytes.Slice(2, 2).ToUShort());
        }

        public int GetPacketSize(Span<byte> headerBytes)
        {
            return BitConverter.ToUInt16(headerBytes.Slice(0, 2));
        }
    }
}
