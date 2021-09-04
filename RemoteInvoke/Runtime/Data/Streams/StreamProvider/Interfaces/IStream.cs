using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteInvoke.Runtime.Data
{
    public interface IStream<T>
    {
        bool CanRead { get; }
        bool CanWrite { get; }
        bool DataAvailable { get; }

        int Read(Span<T> buffer);
        int Read(T[] buffer, int offset, int count);
        void Write(T[] buffer, int offset, int count);
        void Write(T[] buffer);
        void Write(ReadOnlySpan<T> buffer);

        void Close();
    }
}
