using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public interface IPointerProvider
    {
        INetPtr Create(ushort typeId, ushort instanceId);
        INetPtr<T> Create<T>(ushort typeId, ushort instanceId, Action<INetPtr<T>, T> setter, Func<INetPtr<T>, T> getter);
    }
}
