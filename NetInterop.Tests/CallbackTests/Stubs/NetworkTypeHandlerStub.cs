using NetInterop.Abstractions;
using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Tests.CallbackTests.Stubs
{
    public class NetworkTypeHandlerStub : ITypeHandler
    {
        public List<Type> registeredTypes = new List<Type>();
        public bool cleared = false;
        public ISerializableType network = null;
        public IType networkType = null;

        public INetPtr<T> RegisterType<T>(ushort interopId, IActivator<T> activator, IDeactivator<T> deactivator, IPacketSerializer<T> serializer, IPacketDeserializer<T> deserializer)
        {
            throw new NotImplementedException();
        }

        public bool TryGetSerializableType<T>(out ISerializableType<T> netType)
        {
            netType = (ISerializableType<T>)network;
            return true;
        }

        public bool TryGetSerializableType(Type type, out ISerializableType netType)
        {
            netType = network;
            return true;
        }

        public bool TryGetSerializableType(INetPtr typePtr, out ISerializableType netType)
        {
            netType = network;
            return true;
        }

        public bool TryGetSerializableType<T>(INetPtr typePtr, out ISerializableType<T> netType)
        {
            netType = (ISerializableType<T>)network;
            return true;
        }

        public bool TryGetType<T>(out IType<T> netType)
        {
            netType = (IType<T>)networkType;
            return true;
        }

        public bool TryGetType(Type type, out IType netType)
        {
            netType = networkType;
            return true;
        }

        public bool TryGetType(INetPtr typePtr, out IType netType)
        {
            netType = networkType;
            return true;
        }

        public bool TryGetType<T>(INetPtr typePtr, out IType<T> netType)
        {
            netType = (IType<T>)networkType;
            return true;
        }
    }
}
