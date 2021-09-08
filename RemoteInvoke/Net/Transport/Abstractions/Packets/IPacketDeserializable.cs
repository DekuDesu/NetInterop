using RemoteInvoke.Net.Abstractions;
using RemoteInvoke.Net.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteInvoke.Net.Abstractions
{
    public interface IPacketDeserializable<TPacketType, out T> where TPacketType : Enum
    {
        public T Deserialize(Packet<TPacketType> packet);
    }
}
