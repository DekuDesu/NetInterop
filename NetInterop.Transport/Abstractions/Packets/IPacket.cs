using System;

namespace NetInterop.Transport.Core.Abstractions.Packets
{
    public interface IPacket
    {
        /// <summary>
        /// The actual length of the packet including user defined data, reserved, hidden and encoded bytes.
        /// </summary>
        int ActualSize { get; }

        /// <summary>
        /// The length of user defined data appended to the packet
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Returns all the data contained within the packet, including reserved, hidden, and encoded portions
        /// </summary>
        /// <returns></returns>
        byte[] GetData();

        /// <summary>
        /// Appends the data to the end of the packet
        /// </summary>
        /// <param name="newData"></param>
        void Append(byte[] newData);

        /// <summary>
        /// Reserves n <paramref name="length"/> bytes within the packet
        /// </summary>
        /// <param name="length"></param>
        /// <returns><see langword="byte*"/> that points to the start of the reserved space</returns>
        ref byte GetBuffer(int length);

        /// <summary>
        /// </summary>
        /// <returns><see langword="byte*"/> that points to the header of the packet</returns>
        ref byte GetHeaderPointer();

        /// <summary>
        /// Removes n <paramref name="length"/> bytes from the front of the packet
        /// </summary>
        /// <param name="length"></param>
        /// <returns><see langword="byte*"/> that points to the start of the removed portion of the packet</returns>
        ref byte Remove(int length);

        /// <summary>
        /// Compiles and sets the header for this packet so it can be sent
        /// </summary>
        void CompileHeader();
    }
}