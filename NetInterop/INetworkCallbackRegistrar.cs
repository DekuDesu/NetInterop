using NetInterop.Transport.Core.Abstractions.Packets;
using System;

namespace NetInterop
{
    public interface INetworkCallbackRegistrar<TPacket> where TPacket : Enum, IConvertible
    {
        void FreeId(ushort id);
        void Invoke(ushort id, bool responseStatus, IPacket<TPacket> packet);
        void InvokeAndFree(ushort id, bool responseStatus, IPacket<TPacket> packet);
        ushort Register(Action<bool, IPacket<TPacket>> invokable);
        ushort Register(Action callback);
        ushort Register(Action<bool> callback);
    }
}