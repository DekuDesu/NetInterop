using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NetInterop.Abstractions;

namespace NetInterop.Runtime
{
    public class ObjectHeap<T> : IObjectHeap<T>
    {
        private readonly IPointerProvider pointerProvider;
        private readonly INetType<T> type;
        private readonly IProducerConsumerCollection<ushort> availableIndices = new ConcurrentBag<ushort>();
        private readonly SemaphoreSlim locker = new SemaphoreSlim(1, 1);
        private readonly T[] heap = new T[ushort.MaxValue];
        private ushort nextNewIndex = 1;

        public ObjectHeap(INetType<T> type, IPointerProvider pointerProvider)
        {
            this.type = type ?? throw new ArgumentNullException(nameof(type));
            this.pointerProvider = pointerProvider ?? throw new ArgumentNullException(nameof(pointerProvider));
        }

        public INetPtr Alloc(INetPtr ptr)
        {
            T instance = type.Activate();

            ushort address = ptr?.PtrAddress ?? GetNewAddress();

            Set((heap) => heap[address] = instance);

            return pointerProvider.Create(type.TypePointer.PtrType, address);
        }

        public void Free(INetPtr instancePtr)
        {
            Set((heap) =>
            {
                type.Deactivate(ref heap[instancePtr.PtrAddress]);
            });
        }

        public T Get(INetPtr instancePtr) => Get((heap) => heap[instancePtr.PtrAddress]);

        public void Set(INetPtr instancePtr, T value) => Set((heap) => heap[instancePtr.PtrAddress] = value);

        public void Clear()
        {
            Set((heap) =>
            {
                for (int i = 0; i < this.heap.Length; i++)
                {
                    type.Deactivate(ref heap[i]);
                }
            });
        }

        private ushort GetNewAddress()
        {
            if (availableIndices.Count == 0)
            {
                return nextNewIndex++;
            }

            if (availableIndices.TryTake(out ushort newId))
            {
                return newId;
            }

            return nextNewIndex++;
        }

        private void Set(Action<T[]> expression)
        {
            locker.Wait();
            try
            {
                expression.Invoke(heap);
            }
            finally
            {
                locker.Release();
            }
        }

        private T Get(Func<T[], T> expression)
        {
            locker.Wait();
            try
            {
                return expression.Invoke(heap);
            }
            finally
            {
                locker.Release();
            }
        }

        public void Set(INetPtr instancePtr, object value) => Set(instancePtr, (T)value);

        object IObjectHeap.Get(INetPtr instancePtr) => Get(instancePtr);
    }
}
