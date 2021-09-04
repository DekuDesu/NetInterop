using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RemoteInvoke.Runtime.Data
{
    public sealed class DisposableNetworkStream : IStream<byte>
    {
        private readonly NetworkStream stream;

        public DisposableNetworkStream(NetworkStream stream)
        {
            this.stream = stream;
        }

        public bool CanRead => stream.CanRead;

        public bool CanWrite => stream.CanWrite;

        public bool DataAvailable => stream.DataAvailable;

        public void Close() => stream.Close();

        public void Dispose()
        {
            Close();
            stream.Dispose();
        }

        public int Read(byte[] buffer, int offset, int count) => stream.Read(buffer, offset, count);

        public int Read(Span<byte> buffer) => stream.Read(buffer);

        public void Write(byte[] buffer, int offset, int count) => stream.Write(buffer, offset, count);

        public void Write(byte[] buffer) => stream.Write(buffer);

        public void Write(ReadOnlySpan<byte> buffer) => stream.Write(buffer);
    }
}
