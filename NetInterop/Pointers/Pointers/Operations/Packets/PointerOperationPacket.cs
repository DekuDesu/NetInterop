using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class PointerOperationPacket<TPacket> : IPacketSerializable<TPacket> where TPacket : Enum, IConvertible
    {
        private readonly IPacketSerializable<TPacket> wrappedPacket;
        private readonly PointerOperations operation;

        /// <summary>
        /// Appends the operation to the front of the inner packet
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="wrappedPacket"></param>
        public PointerOperationPacket(PointerOperations operation, IPacketSerializable<TPacket> wrappedPacket)
        {
            this.wrappedPacket = wrappedPacket;
            this.operation = operation;
        }

        public TPacket PacketType => wrappedPacket.PacketType;

        public int EstimatePacketSize() => sizeof(byte) + wrappedPacket.EstimatePacketSize();

        public void Serialize(IPacket packetBuilder)
        {
            packetBuilder.AppendByte((byte)operation);

            wrappedPacket.Serialize(packetBuilder);
        }
    }
}
