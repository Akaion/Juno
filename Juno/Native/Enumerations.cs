using System;

namespace Juno.Native
{
    internal static class Enumerations
    {
        [Flags]
        internal enum MemoryProtection
        {
            ReadWrite = 0x04
        }
    }
}