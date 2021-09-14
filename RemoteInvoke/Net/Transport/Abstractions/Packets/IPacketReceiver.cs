using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteInvoke.Net.Transport.Abstractions
{
    public interface IPacketReceiver<TPacketType> where TPacketType : Enum, IConvertible
    {
        void BeginReceiving(CancellationToken token);
    }
}
