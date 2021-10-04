using NetInterop.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Runtime.TypeHandling
{
    public class RuntimeHeap : IObjectHeap
    {
        private readonly IDictionary<ushort, IObjectHeap> heaps = new ConcurrentDictionary<ushort, IObjectHeap>();

        public RuntimeHeap(IEnumerable<KeyValuePair<INetPtr, IObjectHeap>> heaps)
        {
            foreach (var item in heaps)
            {
                this.heaps.Add(item.Key.PtrType, item.Value);
            }
        }

        public INetPtr Alloc(INetPtr ptr) => heaps[ptr.PtrType].Alloc(ptr);

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

        public void Set(INetPtr instancePtr, object value) => heaps[instancePtr.PtrType].Set(instancePtr, value);
    }
}
