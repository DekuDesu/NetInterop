namespace RemoteInvoke.Runtime.Data
{
    public interface IHeaderParser
    {
        int GetPacketSize(uint header);
        int GetPacketType(uint header);
    }
}