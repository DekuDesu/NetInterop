using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Transport.Core.Abstractions.Packets
{
    /// <summary>
    /// Defines an object that is capable of serializing <typeparamref name="TResult"/> into a <see cref="IPacket{TPacket}"/>
    /// </summary>
    /// <typeparam name="TPacket"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface IPacketSerializer<TPacket, TResult> where TPacket : Enum, IConvertible
    {
        void Serialize(TResult value, IPacket<TPacket> packetBuilder);
    }
}
