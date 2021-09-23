using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public interface ISerializableNetworkType<T> :
        ISerializableNetworkType,
        INetworkType<T>,
        IPacketSerializer<T>,
        IPacketDeserializer<T>
    {
    }
    public interface ISerializableNetworkType :
        INetworkType,
        IPacketSerializer,
        IPacketDeserializer
    {
    }
}
