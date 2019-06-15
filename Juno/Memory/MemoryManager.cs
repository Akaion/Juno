using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using static Juno.Native.PInvoke;
using static Juno.Native.Enumerations;

namespace Juno.Memory
{
    internal static class MemoryManager
    {
        internal static void WriteVirtualMemory(IntPtr baseAddress, byte[] bytesToWrite)
        {
            // Adjust the protection of the virtual memory region to ensure it has write privileges

            if (!VirtualProtect(baseAddress, bytesToWrite.Length, MemoryProtection.ReadWrite, out var oldProtection))
            {
                throw new Win32Exception($"Failed to protect a region of virtual memory with error code {Marshal.GetLastWin32Error()}");
            }

            Marshal.Copy(bytesToWrite, 0, baseAddress, bytesToWrite.Length);
            
            // Restore the original protection of the virtual memory region
            
            if (!VirtualProtect(baseAddress, bytesToWrite.Length, oldProtection, out _))
            {
                throw new Win32Exception($"Failed to protect a region of virtual memory with error code {Marshal.GetLastWin32Error()}");
            }
        }
    }
}