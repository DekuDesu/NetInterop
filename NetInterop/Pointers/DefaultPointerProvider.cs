using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class DefaultPointerProvider : IPointerProvider
    {
        public object AmbiguousDeserialize(IPacket packet) => Deserialize(packet);

        public INetPtr Create(ushort typeId, ushort instanceId) => new NetPtr(typeId, instanceId);

        public INetPtr<T> Create<T>(ushort typeId, ushort instanceId, Action<INetPtr<T>, T> setter, Func<INetPtr<T>, T> getter)
        {
            throw new NotImplementedException();
        }

        public INetPtr Deserialize(IPacket packet) => new NetPtr(packet.GetUShort(), packet.GetUShort());

        public void Serialize(INetPtr value, IPacket packetBuilder) => packetBuilder.AppendSerializable(value);
    }
}
