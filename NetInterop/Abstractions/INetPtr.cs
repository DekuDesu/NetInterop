using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NetInterop
{
    public interface INetPtr
    {
        ushort PtrType { get; }
        ushort InstancePtr { get; }
    }
    public interface INetPtr<T> : INetPtr
    {
        T Value { get; set; }
    }
}
