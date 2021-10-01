using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public interface IPointerResponseHandler
    {
        void Handle(bool goodResponse, IPacket packet);
    }
}
