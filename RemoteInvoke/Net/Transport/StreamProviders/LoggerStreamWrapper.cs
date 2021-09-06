using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteInvoke.Net.Transport
{
    public class LoggerStreamWrapper : IStream<byte>
    {
        private readonly IStream<byte> stream;

        public string MessagePrefix { get; set; } = string.Empty;

        public LoggerStreamWrapper(IStream<byte> stream)
        {
            this.stream = stream;
        }

        public bool CanRead => stream.CanRead;
        public bool CanWrite => stream.CanWrite;
        public bool DataAvailable => stream.DataAvailable;

        public void Close()
        {
            Console.WriteLine($"{MessagePrefix}Closing stream");

            stream.Close();

            Console.WriteLine($"{MessagePrefix}Stream closed");
        }

        public int Read(Span<byte> buffer)
        {
            Console.WriteLine($"{MessagePrefix}Reading stream buffer");

            try
            {
                int read = stream.Read(buffer);

                Console.WriteLine($"{MessagePrefix}{read} bytes read from stream");

                return read;
            }
            catch (IOException e)
            {
                Console.WriteLine($"{MessagePrefix}Encountered error while reading from stream\n\t{e.Message}");
            }

            return 0;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            Console.WriteLine($"{MessagePrefix}Reading stream buffer with offset({offset}) count({count})");

            try
            {
                int read = stream.Read(buffer, offset, count);

                Console.WriteLine($"{MessagePrefix}{read} bytes read from stream");

                return read;
            }
            catch (IOException e)
            {
                Console.WriteLine($"{MessagePrefix}Encountered error while reading from stream\n\t{e.Message}");
            }

            return 0;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            Console.WriteLine($"{MessagePrefix}Writing to stream with offset({offset}) count({count})");

            try
            {
                stream.Write(buffer, offset, count);
            }
            catch (IOException e)
            {
                Console.WriteLine($"{MessagePrefix}Encountered error while writing to stream\n\t{e.Message}");
            }

            Console.WriteLine($"{MessagePrefix}Stream writing finished");
        }

        public void Write(byte[] buffer)
        {
            Console.WriteLine($"{MessagePrefix}Writing to stream max length {buffer.Length}");

            try
            {
                stream.Write(buffer);
            }
            catch (IOException e)
            {
                Console.WriteLine($"{MessagePrefix}Encountered error while writing to stream\n\t{e.Message}");
            }

            Console.WriteLine($"{MessagePrefix}Stream writing finished");
        }

        public void Write(ReadOnlySpan<byte> buffer)
        {
            Console.WriteLine($"{MessagePrefix}Writing to stream max length {buffer.Length}");

            try
            {
                stream.Write(buffer);
            }
            catch (IOException e)
            {
                Console.WriteLine($"{MessagePrefix}Encountered error while writing to stream\n\t{e.Message}");
            }

            Console.WriteLine($"{MessagePrefix}Stream writing finished");
        }
    }
}
