using RemoteInvoke.Runtime.NetPtr.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteInvoke.Runtime.Net_Pointer
{
    public struct NetPtr
    {
        public NetPtr(ulong ptr)
        {
            NullPointerCheck(ptr);
            this.Ptr = ptr;
        }

        public ulong Ptr { get; set; }

        /// <summary>
        /// A Null Network Pointer
        /// </summary>
        public static readonly NetPtr NullPointer = NullPtr;

        /// <summary>
        /// Returns the next Property, Field, or Method for the underlying type of this pointer
        /// </summary>
        public NetPtr NextMember => NullPointerCheck(Ptr + memberOffset);
        public NetPtr PreviousMember => NullPointerCheck(Ptr - memberOffset);

        public NetPtr NextElement => NullPointerCheck(Ptr + indexOffset);
        public NetPtr PreviousElement => NullPointerCheck(Ptr - indexOffset);

        public bool PointsToMember => IsInstanceMember(Ptr);
        public bool PointsToUnmanaged => IsUnmanaged(Ptr);
        public bool PointsToArrayElement => IsArrayElement(Ptr);
        public bool PointsToUnmanagedArray => IsUnmanagedArray(Ptr);
        public bool PointsWithinInstance => HasLeadingBits(Ptr) && HasLowerBits(Ptr);
        public bool PointsToInstance => IsInstanceReference(Ptr);


        private const ulong instanceOffset = 0x0001000000000000;
        private const ulong memberOffset = 0x0000000100000000;
        private const ulong indexOffset = 0x0000000000000001;

        private const ulong NullPtr = 0x000000000000;

        private const ulong midMask = 0xFFFF0000FFFFFFFF;
        private const ulong lowerMask = 0xFFFF000000000000;
        private const ulong trailingMask = 0xFFFFFFFF00000000;
        private const int upperOffset = 48;

        private static bool HasLeadingBits(ulong ptr)
        {
            return (ptr >> upperOffset) != 0;
        }

        private static bool HasLowerBits(ulong ptr)
        {
            return ((ptr | lowerMask) ^ lowerMask) != 0;
        }

        private static bool HasMidBits(ulong ptr)
        {
            return ((ptr | midMask) ^ midMask) != 0;
        }

        private static bool HasTrailingBits(ulong ptr)
        {
            return ((ptr | trailingMask) ^ trailingMask) != 0;
        }

        public static bool IsInstanceReference(ulong ptr) => HasLeadingBits(ptr) && HasLowerBits(ptr) is false;

        public static bool IsInstanceMember(ulong ptr) => HasLeadingBits(ptr) && HasMidBits(ptr);

        public static bool IsArrayElement(ulong ptr) => (IsInstanceMember(ptr) && HasTrailingBits(ptr)) || IsUnmanagedArray(ptr);

        public static bool IsUnmanaged(ulong ptr) => HasLeadingBits(ptr) is false && HasMidBits(ptr) is false && HasTrailingBits(ptr);

        public static bool IsUnmanagedArray(ulong ptr) => HasLeadingBits(ptr) is false && HasMidBits(ptr);

        /// <summary>
        /// Throws an exception if the pointer is garunteed to point to a <see langword="null"/>, or most likely <see langword="null"/> pointer
        /// </summary>
        /// <exception cref="Exceptions.NullNetworkPointerException"></exception>
        private static NetPtr NullPointerCheck(ulong value)
        {
            ulong nextInstance = value + instanceOffset;
            ulong previousInstance = value - instanceOffset;

            if (value >= nextInstance || value <= previousInstance || value == NullPointer)
            {
                throw new Exceptions.NullNetworkPointerException(value.ToString());
            }

            return value;
        }

        public static NetPtr operator ++(NetPtr self)
        {
            return self.Ptr + 1;
        }

        public static NetPtr operator --(NetPtr self)
        {
            return self.Ptr + 1;
        }

        public static NetPtr operator >>(NetPtr self, int offset)
        {
            return self.Ptr >> offset;
        }

        public static NetPtr operator <<(NetPtr self, int offset)
        {
            return self.Ptr << offset;
        }

        public static implicit operator ulong(NetPtr other)
        {
            return other.Ptr;
        }

        public static implicit operator NetPtr(ulong ptr)
        {
            return new(ptr);
        }

        public override string ToString()
        {
            return $"{Ptr:X}";
        }
    }
}
