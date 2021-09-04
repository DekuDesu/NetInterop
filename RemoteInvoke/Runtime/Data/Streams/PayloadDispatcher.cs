using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RemoteInvoke.Runtime.Data.Helpers;
#nullable enable
namespace RemoteInvoke.Runtime.Data
{
    public class PayloadDispatcher : IPayloadDispatcher
    {
        private readonly IStream<byte> stream;

        private volatile bool breakSentinel;

        public bool Dispatching { get; private set; }

        public int PollingRate { get; set; } = 10;

        public PayloadDispatcher(IStream<byte> stream)
        {
            this.stream = stream;
        }

        public T? DispatchPayload<T>(Func<Stream, T> Converter)
        {
            return DispatchPayload<T>(Converter, CancellationToken.None);
        }

        public T? DispatchPayload<T>(Func<Stream, T> Converter, CancellationToken token)
        {
            Stream? dispatched = DispatchPayload(token);

            if (dispatched is not null)
            {
                try
                {
                    return Converter(dispatched);
                }
                finally
                {
                    dispatched.Dispose();
                }
            }

            return default;
        }

        public Stream? DispatchPayload()
        {
            return DispatchPayload(CancellationToken.None);
        }

        public Stream? DispatchPayload(CancellationToken token)
        {
            while (true)
            {
                if (breakSentinel)
                {
                    break;
                }

                if (token.IsCancellationRequested)
                {
                    break;
                }

                // check to see if there is anything to dispatch
                if (stream.DataAvailable)
                {
                    // get size of payload to be expected
                    int expectedSize = stream.ReadInt();

                    // copy the bytes to a new stream 
                    return GenerateStream(expectedSize);
                }

                Thread.Sleep(PollingRate);
            }

            return null;
        }

        public void BeginDispatchingPayloads(Action<Stream> payloadHandler, CancellationToken token)
        {
            if (Dispatching) return;

            Dispatching = true;

            while (true)
            {
                if (breakSentinel)
                {
                    break;
                }

                if (token.IsCancellationRequested)
                {
                    break;
                }

                // check to see if there is anything to dispatch
                if (stream.DataAvailable)
                {

                    // get size of payload to be expected
                    int expectedSize = stream.ReadInt();

                    // copy the bytes to a new stream 
                    Stream payloadStream = GenerateStream(expectedSize);

                    // send the stream to the handler
                    payloadHandler(payloadStream);

                    // don't sleep if we just finished, there is likely to be more payloads after this
                    continue;
                }

                Thread.Sleep(PollingRate);
            }

            Dispatching = false;
        }

        public void Cancel(CancellationToken token)
        {
            breakSentinel = true;

            while (Dispatching)
            {
                token.ThrowIfCancellationRequested();
                Thread.Sleep(10);
            }

            breakSentinel = false;
        }

        private Stream GenerateStream(int length)
        {
            MemoryStream newStream = new(length);

            stream.CopyTo(newStream, length);

            newStream.Position = 0;

            return newStream;
        }
    }
}
