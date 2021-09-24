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
        private readonly ISerializableNetworkType<T> serializableNetworkType;

        public SetPointerPacket(INetPtr<T> ptr, T value, ISerializableNetworkType<T> serializableNetworkType)
        {
            this.ptr = ptr;
            this.value = value;
            this.serializableNetworkType = serializableNetworkType ?? throw new ArgumentNullException(nameof(serializableNetworkType));
        }

        public int EstimatePacketSize() => sizeof(int);

        public void Serialize(IPacket packetBuilder)
        {
            packetBuilder.AppendSerializable(ptr);
            serializableNetworkType.Serialize(value, packetBuilder);
        }
    }
}
