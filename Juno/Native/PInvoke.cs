using System;
using System.Runtime.InteropServices;
using static Juno.Native.Enumerations;

namespace Juno.Native
{
    internal static class PInvoke
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool VirtualProtect(IntPtr baseAddress, int protectionSize, MemoryProtection protectionType, out MemoryProtection oldProtectionType);
    }
}