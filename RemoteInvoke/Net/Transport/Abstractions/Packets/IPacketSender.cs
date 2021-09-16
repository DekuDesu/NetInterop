using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteInvoke.Net.Transport.Abstractions
{
    public interface IPacketSender<TPacket> where TPacket : Enum, IConvertible
    {
        public void Send(IPacketSerializable<TPacket> value);

        public void Send(Packet<TPacket> packet);

        public void Send(TPacket packetType, Span<byte> data);
    }
}
