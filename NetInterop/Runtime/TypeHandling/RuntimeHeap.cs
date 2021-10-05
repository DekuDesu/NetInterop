using NetInterop.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Runtime.TypeHandling
{
    public class RuntimeHeap : IObjectHeap
    {
        private readonly INetTypeHandler typeHandler;
        private readonly IPointerProvider pointerProvider;
        private readonly IDictionary<ushort, IObjectHeap> heaps = new ConcurrentDictionary<ushort, IObjectHeap>();

        public RuntimeHeap(INetTypeHandler typeHandler, IPointerProvider pointerProvider)
        {
            this.typeHandler = typeHandler ?? throw new ArgumentNullException(nameof(typeHandler));
            this.pointerProvider = pointerProvider ?? throw new ArgumentNullException(nameof(pointerProvider));
        }

        public INetPtr Alloc(INetPtr ptr)
        {
            // check to see if the type already has a heap
            if (heaps.ContainsKey(ptr.PtrType))
            { 
                return heaps[ptr.PtrType].Alloc(ptr);
            }

            // since it doesnt have a heap attempt to create one if we have enough information
            if (typeHandler.TryGetType(ptr, out INetType type) is false)
            {
                throw new ArgumentException($"Failed to activate a new instance of {ptr} becuase no IObjectHEap with that type is already registered and the type was not found within the type handler. Use ITypeHandler.Register<T> to register the type before attempting to create a new instance of it.");
            }

            // create a new heap
            heaps.Add(ptr.PtrType,Create(type));

            // recurse to return the new value
            return Alloc(ptr);
        }

        public INetPtr<T> Alloc<T>(INetPtr<T> ptr) => Alloc((INetPtr)ptr).As<T>();

        public void Clear()
        {
            foreach (var item in heaps)
            {
                item.Value.Clear();
            }
            heaps.Clear();
        }

        public void Free(INetPtr instancePtr) => heaps[instancePtr.PtrType].Free(instancePtr);

        public object Get(INetPtr instancePtr) => heaps[instancePtr.PtrType].Get(instancePtr);

        public T Get<T>(INetPtr<T> instancePtr) => (T)(Get((INetPtr)instancePtr) ?? default(T));

        public void Set(INetPtr instancePtr, object value) => heaps[instancePtr.PtrType].Set(instancePtr, value);

        public void Set<T>(INetPtr<T> instancePtr, T value) => Set((INetPtr) instancePtr, value);

        private IObjectHeap Create(INetType type)
        {
            return (IObjectHeap)Activator.CreateInstance(typeof(ObjectHeap<>).MakeGenericType(type.BackingType),type,pointerProvider);
        }
    }
}
