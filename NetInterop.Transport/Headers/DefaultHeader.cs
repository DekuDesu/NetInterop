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
    public unsafe class DefaultHeader<TPacket> : IPacketHeader<TPacket> where TPacket : Enum, IConvertible
    {
        private readonly Func<int, TPacket> converter;
        private readonly Func<TPacket, ushort> toNumberConverter;

        public DefaultHeader(Func<int, TPacket> converter = null, Func<TPacket, ushort> toNumberConverter = null)
        {
            this.converter = converter ?? ((num) => (TPacket)Enum.ToObject(typeof(TPacket), num));
            this.toNumberConverter = toNumberConverter ?? ((packetType) => packetType.ToUInt16(null));
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

        public void CreateHeader(IPacket<TPacket> packet)
        {
            ref byte headerPtr = ref packet.GetHeader();

            headerPtr.Write((ushort)packet.Length);

            ushort packetType = toNumberConverter(packet.PacketType); ;

            fixed (byte* p = &headerPtr)
            {
                byte* headerOffsetPtr = p;
                byte* packetTypePtr = (byte*)&packetType;

                // packet type should come after the message size
                headerOffsetPtr += 2;

                *headerOffsetPtr = *packetTypePtr;

                headerOffsetPtr++;
                packetTypePtr++;

                *headerOffsetPtr = *packetTypePtr;
            }
        }

        public TPacket GetHeaderType(ref byte headerPtr)
        {
            fixed (byte* ptr = &headerPtr)
            {
                byte* p = ptr;

                p += 2;

                ushort result = *(ushort*)p;

                return converter(result);
            }
        }

        public int GetPacketSize(ref byte headerPtr)
        {
            fixed (byte* ptr = &headerPtr)
            {
                byte* p = ptr;

                ushort* result = (ushort*)p;

                return (int)*result;
            }
        }
    }
}
