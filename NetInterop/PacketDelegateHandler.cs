using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop
{
    public class PacketDelegateHandler<TPacket> : IDelegateHandler<bool, IPacket<TPacket>> where TPacket : Enum, IConvertible
    {
        private readonly IDictionary<ushort, Action<bool, IPacket<TPacket>>> callbacks = new ConcurrentDictionary<ushort, Action<bool, IPacket<TPacket>>>();
        private ushort nextId = 0;
        private readonly ConcurrentBag<ushort> freedIds = new ConcurrentBag<ushort>();

        public ushort Register(Action<bool, IPacket<TPacket>> callback)
        {
            ushort id = GetId();

            callbacks.Add(id, callback);

            return id;
        }

        public ushort Register(Action<bool> invokable) => Register((boolArg, packetArg) => invokable(boolArg));

        ushort IDelegateHandler.Register<T>(T invokable) => Register((boolArg, packetArg) => invokable.DynamicInvoke());

        public void Invoke(ushort id)
        {
            if (callbacks.ContainsKey(id))
            {
                var invokable = callbacks[id];

                invokable(false, null);
            }
        }

        public void Invoke(ushort id, bool arg, IPacket<TPacket> arg1)
        {
            if (callbacks.ContainsKey(id))
            {
                var invokable = callbacks[id];

                Remove(id);

                invokable(arg, arg1);
            }
        }

        public void Invoke(ushort id, bool arg) => Invoke(id, arg, null);

        private void Remove(ushort id)
        {
            freedIds.Add(id);
            callbacks.Remove(id);
        }

        private ushort GetId()
        {
            if (freedIds.IsEmpty)
            {
                return nextId += (ushort)1;
            }

            if (freedIds.TryTake(out ushort newId))
            {
                return newId;
            }

            return nextId += (ushort)1;
        }
    }
}
