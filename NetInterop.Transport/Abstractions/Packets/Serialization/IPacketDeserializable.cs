using NetInterop.Transport.Core.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Transport.Core.Abstractions.Packets
{
    public interface IPacketDeserializable<out T>
    {
        T Deserialize(IPacket packet);
    }
}
