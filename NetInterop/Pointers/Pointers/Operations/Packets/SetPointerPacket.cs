using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class SetPointerPacket<T> : IPacketSerializable
    {
        private readonly INetPtr<T> ptr;
        private readonly T value;
        private readonly IPacketSerializer<T> serializableNetworkType;

        public SetPointerPacket(INetPtr<T> ptr, T value, IPacketSerializer<T> serializableNetworkType)
        {
            this.ptr = ptr;
            this.value = value;
            this.serializableNetworkType = serializableNetworkType ?? throw new ArgumentNullException(nameof(serializableNetworkType));
        }

        public int EstimatePacketSize() => sizeof(int) + serializableNetworkType.EstimatePacketSize(value);

        public void Serialize(IPacket packetBuilder)
        {
            packetBuilder.AppendSerializable(ptr);
            serializableNetworkType.Serialize(value, packetBuilder);
        }
    }
    public class SetAmbiguousPointerPacket : IPacketSerializable
    {
        private readonly INetPtr ptr;
        private readonly object value;
        private readonly IPacketSerializer serializableNetworkType;

        public SetAmbiguousPointerPacket(INetPtr ptr, object value, IPacketSerializer serializableNetworkType)
        {
            this.ptr = ptr;
            this.value = value;
            this.serializableNetworkType = serializableNetworkType ?? throw new ArgumentNullException(nameof(serializableNetworkType));
        }

        public int EstimatePacketSize() => sizeof(int) + serializableNetworkType.EstimatePacketSize(value);

        public void Serialize(IPacket packetBuilder)
        {
            packetBuilder.AppendSerializable(ptr);
            serializableNetworkType.AmbiguousSerialize(value, packetBuilder);
        }
    }
}
