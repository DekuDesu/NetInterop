using NetInterop.Transport.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class DependencyModifierStub<T> : IDependencyModifier<T>
    {
        public T Inject(T value) => value;
    }
}
