using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Concurrent;

namespace NetInterop
{
    public class RegisteredNetworkType<T> : INetworkType, INetworkType<T>
    {
        private readonly IPointerProvider pointerProvider;
        private readonly Func<T> instantiator;
        private readonly bool isDisposable;
        private ConcurrentBag<ushort> freedIds = new ConcurrentBag<ushort>();
        private readonly Action<T> disposer;

        private ushort instanceIndex = 0;
        private T[] instances = new T[ushort.MaxValue];
        private object locker = new object();

        public ushort Id { get; set; }

        public RegisteredNetworkType(ushort id, Func<T> instantiator = null, Action<T> disposer = null)
        {
            this.instantiator = instantiator ?? DefaultInstantiator;
            this.Id = id;
            isDisposable = typeof(T).GetInterface(nameof(IDisposable)) != null;
            this.disposer = disposer;
        }

        public INetPtr AllocPtr()
        {
            ushort instance = GetNewAddress();

            T newInstance = instantiator();

            lock (locker)
            {
                instances[instance] = newInstance;
            }

            return pointerProvider.Create<T>(Id, instance, (ptr, value) => SetPtr(ptr, value), (ptr) => GetPtr(ptr));
        }

        public T GetPtr(INetPtr<T> ptr)
        {
            lock (locker)
            {
                return instances[ptr.InstancePtr];
            }
        }

        public void SetPtr(INetPtr ptr, object value)
        {
            if (ptr.PtrType == 0)
            {
                throw new NullReferenceException($"Provided ptr was null: {ptr}");
            }
            if (value is T isT)
            {
                SetPtr(ptr, isT);
            }
            else
            {
                throw new InvalidCastException($"Failed to cast pointer {ptr} to {typeof(T).FullName}, expected type {this.Id} got {ptr.PtrType} ({value})");
            }
        }

        public void SetPtr(INetPtr<T> ptr, T value)
        {
            lock (locker)
            {
                instances[ptr.InstancePtr] = value;
            }
        }

        object INetworkType.GetPtr(INetPtr ptr)
        {
            lock (locker)
            {
                return instances[ptr.InstancePtr];
            }
        }

        public void Free(INetPtr ptr)
        {
            if (ptr is INetPtr<T> isTypedNetPtr)
            {
                DisposeManagedT(this.GetPtr(isTypedNetPtr));
            }


            freedIds.Add(ptr.InstancePtr);
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
