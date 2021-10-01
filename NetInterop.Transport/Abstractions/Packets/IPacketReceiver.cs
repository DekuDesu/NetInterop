using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetInterop.Transport.Core.Abstractions.Packets
{
    /// <summary>
    /// Defines an object that determines if data is available, and if it is, is incharge of sending the data and subsequent packets to an <see cref="IPacketDispatcher"/> to be dispatched to <see cref="IPacketHandler"/>s
    /// </summary>
    public interface IPacketReceiver
    {
        /// <summary>
        /// Blocking; Checks if data is available synchronously, if data is available a single packet is dispatched
        /// </summary>
        /// <returns><see langword="true"/> if a packet was dispatched, otherwise false</returns>
        bool Receive();

        /// <summary>
        /// Blocking; Checks if data is available synchronously, if data is available a packet is dispatched. Polls for available data every 1ms asynchronously
        /// </summary>
        void ReceiveMany(CancellationToken token);

        /// <summary>
        /// Blocking; Checks if data is available synchronously, if data is available a packet is dispatched. No delay in polling.
        /// </summary>
        /// <returns><see langword="true"/> if a packet was dispatched during the iteration, otherwise false</returns>
        IEnumerator<bool> ReceiveMany();

        /// <summary>
        /// Blocking async; Checks if data is available asynchronously, if data is available a packet is dispatched. Polls for available data every 1ms asynchronously
        /// </summary>
        Task ReceiveAsync(CancellationToken token);

        /// <summary>
        /// Timer based; Polls connection to determine if data is available every 1/60th of a second. Dispatches packet if data is available on timer pool thread.
        /// </summary>
        void BeginReceiving();

        /// <summary>
        /// Timer based; stops the timer-thread from polling the connection
        /// </summary>
        void StopReceiving();
    }
}
