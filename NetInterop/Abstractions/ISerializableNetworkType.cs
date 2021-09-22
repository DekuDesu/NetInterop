using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public interface ISerializableNetworkType<T> :
        INetworkType<T>,
        IPacketSerializer<T>,
        IPacketSerializer,
        IPacketDeserializer<T>,
        IPacketDeserializer
    {
    }
}
