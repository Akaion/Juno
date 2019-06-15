using System;
using System.Collections.Generic;

namespace Juno.Assembly
{
    internal static class Assembler
    {
        internal static byte[] AssembleJump(IntPtr jumpAddress)
        {
            var shellcode = new List<byte>();

            if (Environment.Is64BitProcess)
            {
                // mov rax, jumpAddress
                
                shellcode.AddRange(new byte[] {0x48, 0xB8});
                
                shellcode.AddRange(BitConverter.GetBytes((ulong) jumpAddress));
                
                // jmp rax
                
                shellcode.AddRange(new byte[] {0xFF, 0xE0});
            }

            else
            {
                // mov eax, jumpAddress
                
                shellcode.Add(0xB8);
                
                shellcode.AddRange(BitConverter.GetBytes((uint) jumpAddress));
                
                // jmp eax
                
                shellcode.AddRange(new byte[] {0xFF, 0xE0});
            }
            
            return shellcode.ToArray();
        }
    }
}