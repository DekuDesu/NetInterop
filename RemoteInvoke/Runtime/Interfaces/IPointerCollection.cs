using System.Threading.Tasks;

namespace RemoteInvoke.Runtime
{
    public interface IPointerCollection
    {
        int Capacity { get; }
        int Count { get; }

        int Allocate<T>(T instance);
        Task<int> AllocateAsync<T>(T instance);

        void Free(int ptr);
        Task FreeAsync(int ptr);

        T Read<T>(int ptr);
        Task<T> ReadAsync<T>(int ptr);

        void Write<T>(int ptr, T value);
        Task WriteAsync<T>(int ptr, T value);
    }
}