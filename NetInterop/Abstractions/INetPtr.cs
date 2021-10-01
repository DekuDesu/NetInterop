using NetInterop.Attributes;
using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NetInterop
{
    [InteropId(0xFFFF)]
    public interface INetPtr : IPacketSerializable
    {
        ushort PtrType { get; }
        ushort PtrAddress { get; }
        INetPtr<T> As<T>();
    }
    public interface INetPtr<T> : INetPtr
    {
        T Value { get; set; }
    }
}
