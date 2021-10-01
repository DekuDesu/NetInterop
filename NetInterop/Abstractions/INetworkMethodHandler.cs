using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NetInterop
{
    public interface INetworkMethodHandler
    {
        INetPtr Register(Delegate method);
        INetPtr Register(MethodInfo method);
        void Invoke(INetPtr methodPtr, IPacket packet);
        void Invoke(INetPtr methodPtr, IPacket packet, IPacket packetBuilder);
        bool TryGetSerializer(INetPtr ptr, out IPacketSerializer<object[]> serializer);
        bool TryGetDeserializer(INetPtr ptr, out IPacketDeserializer<object[]> deserializer);
        void Clear();
    }
}
