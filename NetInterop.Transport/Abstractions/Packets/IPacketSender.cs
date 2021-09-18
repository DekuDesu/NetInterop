using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetInterop.Transport.Core.Packets;

namespace NetInterop.Transport.Core.Abstractions.Packets
{
    public interface IPacketSender<TPacket> where TPacket : Enum, IConvertible
    {
        public void Send(IPacketSerializable<TPacket> value);

        public void Send(IPacket<TPacket> packet);

        public void Send(TPacket packetType, byte[] data);
    }
}
