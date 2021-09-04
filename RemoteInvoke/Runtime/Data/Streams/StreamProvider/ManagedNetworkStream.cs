using RemoteInvoke.Net.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RemoteInvoke.Runtime.Data
{
    /// <summary>
    /// Represents a <see cref="NetworkStream"/> who's lifetime is managed by another object
    /// </summary>
    public class ManagedNetworkStream : IStream<byte>
    {
        private readonly IClient<NetworkStream> client;

        public ManagedNetworkStream(IClient<NetworkStream> client)
        {
            this.client = client;
        }

        public bool CanRead => client.Available && client.Client.CanRead;
        public bool CanWrite => client.Available && client.Client.CanWrite;
        public bool DataAvailable => client.Available && client.Client.DataAvailable;

        public void Close()
        {
            // our stream is managed by the client, we should not dispose or close it ourselfs
            if (client.Connected)
            {
                client.Disconnect();
            }
        }


        public int Read(byte[] buffer, int offset, int count)
        {
            if (CanRead)
            {
                return client.Client.Read(buffer, offset, count);
            }
            return 0;
        }

        public int Read(Span<byte> buffer)
        {
            if (CanRead)
            {
                return client.Client.Read(buffer);
            }
            return 0;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            if (CanWrite)
            {
                client.Client.Write(buffer, offset, count);
            }
        }

        public void Write(byte[] buffer)
        {
            if (CanWrite)
            {
                client.Client.Write(buffer);
            }
        }

        public void Write(ReadOnlySpan<byte> buffer)
        {
            if (CanWrite)
            {
                client.Client.Write(buffer);
            }
        }
    }
}
