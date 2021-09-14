using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteInvoke.Net.Transport.Packets
{
    public enum PacketTypes : byte
    {
        none,
        IsSignedFlag = 0b_1000_0000,
        IsBigEndianFlag = 0b_0100_0000,
        IsPointerFlag = 0b_0010_0000,
        SetValueFlag = 0b_0001_0000,
        Bool = 0x01,
        Byte = 0x02,
        Char = 0x03,
        Short = 0x04,
        Int = 0x05,
        Long = 0x06,
        Float = 0x07,
        Double = 0x08,
        Decimal = 0x09,
        String = 0x0A,
        DateTime = 0x0B,
        Unmanaged = 0x0C,
        Complex = 0x0D,
    }
}
