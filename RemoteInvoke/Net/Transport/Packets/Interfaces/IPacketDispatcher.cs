using RemoteInvoke.Net.Abstractions;
using System;
using System.Threading;

namespace RemoteInvoke.Net.Transport.Packets
{
    public interface IPacketDispatcher<T> where T : Enum, IConvertible
    {
        /// <summary>
        /// Checks to see if a data is available, if it is creates and returns new packet.
        /// <para>
        /// Blocking; Blocks while reading data, if available, otherwise Non-Blocking;
        /// </para>
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        bool TryReadPacket(out Packet<T> packet);
        /// <summary>
        /// Attempts to write the packet to the underlying stream, waits until a response packet is received.
        /// <para>
        /// Blocking; Graceful Cancel;
        /// </para>
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="responsePacket"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        bool TryWritePacket(Packet<T> packet, out Packet<T> responsePacket, CancellationToken token = default);
        /// <summary>
        /// Waits for packet to be received
        /// <para>
        /// Blocking; Graceful Cancel;
        /// </para>
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Packet<T> WaitForPacket(CancellationToken token = default);
        /// <summary>
        /// Writes the packet to the underlying stream, does not wait for a response packet to be received
        /// <para>
        /// Blocking; Blocks while writing data to underlying stream, if available, otherwise Non-Blocking;
        /// </para>
        /// </summary>
        /// <param name="packet"></param>
        void WriteBlindPacket(Packet<T> packet);
    }
}