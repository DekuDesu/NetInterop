using System;
using System.IO;
using System.Threading;

namespace RemoteInvoke.Runtime.Data
{
    /// <summary>
    /// Reponsible for waiting for responses from the stream. When a response is received it copies the data into a new stream and dispatches it.
    /// </summary>
    public interface IPayloadDispatcher
    {
        bool Dispatching { get; }
        int PollingRate { get; set; }

        void Cancel(CancellationToken token);

        /// <summary>
        /// Blocking; Waits until data is received, copies the data to a new stream
        /// <para>
        /// Caller manages lifetime of stream
        /// </para>
        /// </summary>
        /// <returns></returns>
        Stream DispatchPayload();

        /// <summary>
        /// Blocking; Waits until data is received, copies the data to a new stream
        /// <para>
        /// Caller manages lifetime of stream
        /// </para>
        /// </summary>
        /// <returns></returns>
        Stream DispatchPayload(CancellationToken token);

        /// <summary>
        /// Blocking; Waits until data is received, copies the data to a new stream, Uses <paramref name="Converter"/> to create destination type
        /// <para>
        /// This object manages the lifetime of the stream, it is disposed after <paramref name="Converter"/> returns;
        /// </para>
        /// </summary>
        /// <returns></returns>
        T? DispatchPayload<T>(Func<Stream, T> Converter);

        /// <summary>
        /// Blocking; Waits until data is received, copies the data to a new stream, Uses <paramref name="Converter"/> to create destination type
        /// <para>
        /// This object manages the lifetime of the stream, it is disposed after <paramref name="Converter"/> returns;
        /// </para>
        /// </summary>
        /// <returns></returns>
        T? DispatchPayload<T>(Func<Stream, T> Converter, CancellationToken token);
        void BeginDispatchingPayloads(Action<Stream> payloadHandler, CancellationToken token);
    }
}