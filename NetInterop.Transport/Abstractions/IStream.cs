using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Transport.Core.Abstractions
{
    public interface IStream<T>
    {
        bool CanRead { get; }
        bool CanWrite { get; }
        bool DataAvailable { get; }

        int Read(T[] buffer, int offset, int count);
        void Write(T[] buffer, int offset, int count);
        void Write(T[] buffer);

        void Close();
    }
}
