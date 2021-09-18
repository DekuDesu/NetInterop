using NetInterop.Transport.Core.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Transport.Core.Abstractions.Packets
{
    public interface IPacketDeserializable<TPacketType, out T> where TPacketType : Enum
    {
        public T Deserialize(IPacket<TPacketType> packet);
    }
}
