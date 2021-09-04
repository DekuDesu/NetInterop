using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteInvoke.Runtime
{
    public class LocalPointerCollection : IPointerCollection
    {
        public const int NullPtr = 0;
        public const int DefaultCapacity = 100;

        private readonly IProducerConsumerCollection<int> freedAllocations = new ConcurrentBag<int>();
        private readonly EventWaitHandle allocationResizeLock = new(true, EventResetMode.ManualReset);
        private object[] allocationBackingArray;

        private volatile int nextAllocationIndex = 1;
        private Memory<object> allocations;

        public int Count => nextAllocationIndex - freedAllocations.Count - 1;

        public int Capacity { get; private set; }

        public LocalPointerCollection(int capacity = DefaultCapacity)
        {
            this.Capacity = capacity;
            allocationBackingArray = new object[capacity];
            allocations = new(allocationBackingArray);
        }

        public int Allocate<T>(T instance)
        {
            int ptr = GetNewPtr();

            SetPtr(ptr, instance);

            return ptr;
        }

        public Task<int> AllocateAsync<T>(T instance)
        {
            return new Task<int>(() => Allocate<T>(instance));
        }

        public void Free(int ptr)
        {
            VerifyPtr(ptr, "Attempted to free a null pointer");

            SetPtr(ptr, null);

            freedAllocations.TryAdd(ptr);
        }

        public Task FreeAsync(int ptr)
        {
            return new Task(() => Free(ptr));
        }

        public T Read<T>(int ptr)
        {
            VerifyPtr(ptr, "Attempted to read the value of a null pointer");

            return (T)GetPtr(ptr);
        }

        public Task<T> ReadAsync<T>(int ptr)
        {
            return new Task<T>(() => Read<T>(ptr));
        }

        public void Write<T>(int ptr, T value)
        {
            VerifyPtr(ptr, "Attempted to write the value of a null pointer");

            SetPtr(ptr, value);
        }

        public Task WriteAsync<T>(int ptr, T value)
        {
            return new Task(() => Write<T>(ptr, value));
        }

        private void VerifyPtr(int ptr, string msg)
        {
            if (ptr <= 0 || ptr > nextAllocationIndex)
            {
                throw new NullReferenceException(msg);
            }
        }

        private int GetNewPtr()
        {
            if (freedAllocations.Count is 0)
            {
                int newPtr = Interlocked.Increment(ref nextAllocationIndex);

                if (newPtr >= allocations.Length)
                {
                    ResizeAllocations();
                }

                return newPtr;
            }
            if (freedAllocations.TryTake(out int ptr))
            {
                return ptr;
            }
            return NullPtr;
        }

        private void SetPtr(int ptr, object value)
        {
            allocationResizeLock.WaitOne();

            allocations.Span[ptr] = value;
        }

        private object GetPtr(int ptr)
        {
            allocationResizeLock.WaitOne();

            return allocations.Span[ptr];
        }

        private void ResizeAllocations()
        {
            allocationResizeLock.WaitOne();
            allocationResizeLock.Reset();

            try
            {
                Array.Resize(ref allocationBackingArray, Capacity += 50);
                allocations = new(allocationBackingArray);
            }
            finally
            {
                allocationResizeLock.Set();
            }
        }
    }
}
