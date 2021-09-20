namespace NetInterop
{
    public interface ITypeSafeNetworkType<T>
    {
        T Reference(ushort ptr);
    }
}