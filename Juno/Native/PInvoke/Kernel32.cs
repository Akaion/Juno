using System;
using System.Runtime.InteropServices;
using Juno.Native.Enumerations;

namespace Juno.Native.PInvoke
{
    internal static class Kernel32
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool VirtualProtect(IntPtr baseAddress, int protectionSize, MemoryProtectionType protectionType, out MemoryProtectionType oldProtectionType);
    }
}