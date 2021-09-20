namespace NetInterop
{
    public interface INetworkType
    {
        int Id { get; }

        ushort Alloc();

        void Free(ushort ptr);

        object Reference(ushort ptr);

        void FreeAll();
    }
}