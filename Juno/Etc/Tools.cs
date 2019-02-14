using System;
using System.Runtime.InteropServices;
using Juno.Services;

namespace Juno.Etc
{
    internal static class Tools
    {
        internal static void InternalWriteMemory(IntPtr address, byte[] buffer)
        {
            // Change the protection of the memory region at the address
            
            if (!Native.VirtualProtect(address, (uint) buffer.Length, (uint) Native.MemoryProtection.ExecuteReadWrite, out var oldProtection))
            {
                ExceptionHandler.ThrowWin32Exception("Failed to protect memory in the process");
            }
            
            // Write the buffer into the memory region
            
            Marshal.Copy(buffer, 0, address, buffer.Length);
            
            // Restore the protection of the memory region at the address
            
            if (!Native.VirtualProtect(address, (uint) buffer.Length, oldProtection, out _))
            {
                ExceptionHandler.ThrowWin32Exception("Failed to protect memory in the process");
            }
        }
    }
}