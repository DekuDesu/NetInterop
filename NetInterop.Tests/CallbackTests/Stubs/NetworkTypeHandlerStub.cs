using NetInterop.Abstractions;
using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Tests.CallbackTests.Stubs
{
    public class NetworkTypeHandlerStub : INetworkTypeHandler
    {
        public List<Type> registeredTypes = new List<Type>();
        public bool cleared = false;
        public ISerializableNetworkType network = null;
        public INetworkType networkType = null;
        public void Clear()
        {
            cleared = true;
        }

        public INetPtr<T> RegisterType<T>(ushort interopId, IActivator<T> activator, IDeactivator<T> disposer, IPacketSerializer<T> serializer, IPacketDeserializer<T> deserializer)
        {
            throw new NotImplementedException();
        }

        public bool TryGetAmbiguousSerializableType(INetPtr id, out ISerializableNetworkType serializableNetworkType)
        {
            serializableNetworkType = network;
            return true;
        }

        public bool TryGetAmbiguousType(INetPtr ptr, out INetworkType type)
        {
            type = network;
            return true;
        }

        public bool TryGetSerializableType<T>(out ISerializableNetworkType<T> serializableNetworkType)
        {
            if (network is ISerializableNetworkType<T> sType)
            {
                serializableNetworkType = sType;
                return true;
            }
            serializableNetworkType = default;
            return false;
        }

        public bool TryGetSerializableType<T>(INetPtr<T> id, out ISerializableNetworkType<T> serializableNetworkType)
        {
            if (network is ISerializableNetworkType<T> sType)
            {
                serializableNetworkType = sType;
                return true;
            }
            serializableNetworkType = default;
            return false;
        }

        public bool TryGetType<T>(INetPtr<T> ptr, out INetworkType<T> type)
        {
            if (networkType is INetworkType<T> t)
            {
                type = t;
                return true;
            }
            type = default;
            return true;
        }

        public bool TryGetType<T>(out INetworkType<T> type)
        {
            if (networkType is INetworkType<T> t)
            {
                type = t;
                return true;
            }
            type = default;
            return true;
        }

        public bool TryGetTypePtr<T>(out INetPtr<T> ptr)
        {
            ptr = new NetPtr<T>(1, 0); ;
            return true;
        }

        public bool TryGetTypePtr(Type type, out INetPtr ptr)
        {
            ptr = new NetPtr(1, 0); ;
            return true;
        }
    }
}
