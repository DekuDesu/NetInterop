namespace RemoteInvoke.Runtime.Data
{
    public interface IHeaderParser
    {
        uint CreateHeader(ushort packetSize, byte packetType);
        uint CreateLargeHeader(int packetSize, byte packetType);
        int GetPacketType(uint header);
        int GetPacketSize(uint header);
    }
}