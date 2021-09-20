using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Concurrent;

namespace NetInterop
{
    public class RegisteredNetworkType<T> : INetworkType, ITypeSafeNetworkType<T>
    {
        private readonly Func<T> instantiator;
        private readonly bool isDisposable;
        private ConcurrentBag<ushort> freedIds = new ConcurrentBag<ushort>();
        private readonly Action<T> disposer;

        private ushort instanceIndex = 0;
        private T[] instances = new T[ushort.MaxValue];
        private object locker = new object();

        public int Id { get; set; }

        public RegisteredNetworkType(int id, Func<T> instantiator = null, Action<T> disposer = null)
        {
            this.instantiator = instantiator ?? DefaultInstantiator;
            this.Id = id;
            isDisposable = typeof(T).GetInterface(nameof(IDisposable)) != null;
            this.disposer = disposer;
        }

        public ushort Alloc()
        {
            ushort id = GetNewAddress();

            T newInstance = instantiator();

            lock (locker)
            {
                instances[id] = newInstance;
            }

            return id;
        }

        public T Reference(ushort ptr)
        {
            lock (locker)
            {
                return instances[ptr];
            }
        }

        object INetworkType.Reference(ushort ptr) => Reference(ptr);

        public void Free(ushort ptr)
        {
            DisposeManagedT(Reference(ptr));

            freedIds.Add(ptr);
        }

        public void FreeAll()
        {
            T[] safeCopy = Array.Empty<T>();

            lock (locker)
            {
                instances.CopyTo(safeCopy, 0);
            }

            for (int i = 0; i < safeCopy.Length; i++)
            {
                DisposeManagedT(safeCopy[i]);
            }

            freedIds = new ConcurrentBag<ushort>();

            lock (locker)
            {
                instances = new T[ushort.MaxValue];
            }
        }

        private ushort GetNewAddress()
        {
            if (freedIds.IsEmpty)
            {
                return instanceIndex++;
            }

            if (freedIds.TryTake(out ushort newId))
            {
                return newId;
            }

            return instanceIndex++;
        }

        private T DefaultInstantiator()
        {
            return Activator.CreateInstance<T>();
        }

        private void DisposeManagedT(T instance)
        {
            if (isDisposable)
            {
                if (instance.Equals(default) is false)
                {
                    if (instance is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }

            if (instance != null)
            {
                disposer?.Invoke(instance);
            }
        }
    }
}
