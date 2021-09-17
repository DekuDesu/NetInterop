using NetInterop.Transport.Core.Abstractions;
using NetInterop.Transport.Core.Abstractions.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Transport.Sockets.Server
{
    public class DefaultTcpStream : IStream<byte>
    {
        private readonly NetworkStream stream;
        private readonly IConnection connection;

        public DefaultTcpStream(TcpClient client, IConnection connection)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            this.stream = client.GetStream();

            if (stream is null)
            {
                throw new ArgumentNullException(nameof(client.GetStream));
            }

            this.connection = connection;
        }

        public bool CanRead => stream.CanRead;
        public bool CanWrite => stream.CanWrite;
        public bool DataAvailable => stream.DataAvailable;

        [DebuggerHidden]
        public void Close() => connection.Disconnect();

        [DebuggerHidden]
        public int Read(Span<byte> buffer) => stream.Read(buffer);

        [DebuggerHidden]
        public int Read(byte[] buffer, int offset, int count) => stream.Read(buffer, offset, count);

        [DebuggerHidden]
        public void Write(byte[] buffer, int offset, int count) => stream.Write(buffer, offset, count);

        [DebuggerHidden]
        public void Write(byte[] buffer) => stream.Write(buffer);

        [DebuggerHidden]
        public void Write(ReadOnlySpan<byte> buffer) => stream.Write(buffer);
    }
}
