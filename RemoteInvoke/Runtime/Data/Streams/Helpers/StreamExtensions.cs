using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteInvoke.Runtime.Data.Helpers
{
    public static class StreamExtensions
    {

        public static void CopyTo(this Stream stream, Stream destination, int count, int bufferSize = 81920, CancellationToken token = default)
        {
            CopyTo(stream.Read, destination.Write, count, bufferSize, token);
        }

        public static void CopyTo(this IStream<byte> stream, IStream<byte> destination, int count, int bufferSize = 81920, CancellationToken token = default)
        {
            CopyTo(stream.Read, destination.Write, count, bufferSize, token);
        }

        public static void CopyTo(this Stream stream, IStream<byte> destination, int count, int bufferSize = 81920, CancellationToken token = default)
        {
            CopyTo(stream.Read, destination.Write, count, bufferSize, token);
        }

        public static void CopyTo(this IStream<byte> stream, Stream destination, int count, int bufferSize = 81920, CancellationToken token = default)
        {
            CopyTo(stream.Read, destination.Write, count, bufferSize, token);
        }

        private static void CopyTo(Func<byte[], int, int, int> reader, Action<byte[], int, int> writer, int count, int bufferSize = 81920, CancellationToken token = default)
        {
            bufferSize = Math.Min(count, bufferSize);

            byte[] buffer = new byte[bufferSize];

            int position = 0;

            while (token.IsCancellationRequested is false)
            {
                if (position >= count)
                {
                    break;
                }

                int read = reader(buffer, 0, bufferSize);

                position += read;

                if (read is 0)
                {
                    break;
                }

                writer(buffer, 0, read);
            }
        }
    }
}
