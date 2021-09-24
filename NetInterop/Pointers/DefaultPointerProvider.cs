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

        public INetPtr<T> Create<T>(ushort typeId, ushort instanceId)
        {
            return new NetPtr<T>(typeId, instanceId);
        }

        public INetPtr Deserialize(IPacket packet)
        {
            try
            {
                return new NetPtr(packet.GetUShort(), packet.GetUShort());
            }
            // IORE is thrown if the packet does not contain enough bytes to form two ushorts
            catch (IndexOutOfRangeException)
            {
                return null;
            }
        }

        public void Serialize(INetPtr value, IPacket packetBuilder) => packetBuilder.AppendSerializable(value);
    }
}
