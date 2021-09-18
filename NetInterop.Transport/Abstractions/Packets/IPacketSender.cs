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
        void Send(IPacketSerializable<TPacket> value);

        void Send(IPacket<TPacket> packet);

        void Send(TPacket packetType, byte[] data);
    }
}
