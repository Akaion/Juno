using System;

namespace Juno.Etc
{
    internal static class Shellcode
    {
        internal static byte[] JumpToFunctionX86(IntPtr functionAddress)
        {
            var shellcode = new byte[]
            {
                0xB8, 0x00, 0x00, 0x00, 0x00, // mov eax, 0x00 (function address)
                0xFF, 0xE0                    // jmp eax
            };
            
            // Get the byte representation of the pointer
            
            var functionAddressBytes = BitConverter.GetBytes((uint) functionAddress);
            
            // Write the pointer into the shellcode
            
            Buffer.BlockCopy(functionAddressBytes, 0, shellcode, 1, 4);
            
            return shellcode;
        }

        internal static byte[] JumpToFunctionX64(IntPtr functionAddress)
        {
            var shellcode = new byte[]
            {
                0x48, 0xB8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // movabs rax, 0x00 (function address)
                0xFF, 0xE0                                                  // jmp rax
            };
            
            // Get the byte representation of the pointer
            
            var functionAddressBytes = BitConverter.GetBytes((ulong) functionAddress);
            
            // Write the pointer into the shellcode
            
            Buffer.BlockCopy(functionAddressBytes, 0, shellcode, 2, 8);
            
            return shellcode;
        }
    }
}