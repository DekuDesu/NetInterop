﻿using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class DefaultPointerPacket<TPacket> : IPacketSerializable<TPacket> where TPacket : Enum, IConvertible
    {
        private readonly PointerOperations operation;
        private readonly INetPtr ptr;
        private readonly IPacketSerializable<TPacket> additionalPacketData;

        public DefaultPointerPacket(PointerOperations operation, INetPtr ptr, IPacketSerializable<TPacket> additionalPacketData = null)
        {
            this.operation = operation;
            this.ptr = ptr;
            this.additionalPacketData = additionalPacketData;
        }

        public TPacket PacketType { get; } = default;

        public int EstimatePacketSize() => 1 + sizeof(ushort) + (ptr.InstancePtr > 0 ? sizeof(ushort) : 0) + (additionalPacketData?.EstimatePacketSize() ?? 0);

        public void Serialize(IPacket<TPacket> packetBuilder)
        {
            packetBuilder.AppendByte((byte)operation);

            packetBuilder.AppendUShort(ptr.PtrType);

            if (ptr.InstancePtr != 0)
            {
                packetBuilder.AppendUShort(ptr.PtrType);
            }
            if (additionalPacketData != null)
            {
                additionalPacketData.Serialize(packetBuilder);
            }
        }
    }
}
