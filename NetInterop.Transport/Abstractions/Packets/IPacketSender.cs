using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetInterop.Transport.Core.Packets;

namespace NetInterop.Transport.Core.Abstractions.Packets
{
    public interface IPacketSender
    {
        void Send(IPacketSerializable value);

        void Send(IPacket packet);
    }
}
