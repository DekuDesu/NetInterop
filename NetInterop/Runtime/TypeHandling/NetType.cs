using System;
using System.Collections.Generic;
using System.Text;
using NetInterop.Abstractions;
using NetInterop.Transport.Core.Abstractions.Packets;

namespace NetInterop.Runtime
{
    public class NetType<T> : IType<T>
    {
        private readonly IActivator<T> activator;
        private readonly IDeactivator<T> deactivator;

        public Type BackingType { get; } = typeof(T);

        public NetType(INetPtr typePointer, IActivator<T> activator, IDeactivator<T> deactivator)
        {
            TypePointer = typePointer ?? throw new ArgumentNullException(nameof(typePointer));
            this.activator = activator ?? throw new ArgumentNullException(nameof(activator));
            this.deactivator = deactivator ?? throw new ArgumentNullException(nameof(deactivator));
        }

        public INetPtr TypePointer { get; private set; }

        public T Activate() => activator.CreateInstance();

        public void Deactivate(ref T instance) => deactivator.DestroyInstance(ref instance);

        public void Deactivate(ref object instance) => deactivator.DestroyInstance(ref instance);

        object IType.Activate() => activator.CreateInstance();
    }
}
