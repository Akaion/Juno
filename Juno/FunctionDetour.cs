using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Juno.Etc;

namespace Juno
{
    public class FunctionDetour <TOriginalClass, TTargetClass>
    {
        private readonly byte[] _detourBytes;
        
        public bool IsHookActive;
        
        private readonly byte[] _originalBytes;
        
        private readonly IntPtr _originalFunctionAddress;
        
        public FunctionDetour(string originalFunction, string targetFunction)
        {
            // Get the information about the original function
            
            var originalFunctionInfo = typeof(TOriginalClass).GetMethod(originalFunction, BindingFlags.Instance | BindingFlags.Public);
            
            if (originalFunctionInfo is null)
            {
                throw new ArgumentException($"No function called {originalFunction} was found in the class {typeof(TOriginalClass)}");
            }
            
            // Ensure the function is JIT compiled
            
            RuntimeHelpers.PrepareMethod(originalFunctionInfo.MethodHandle);
            
            // Get a pointer to the original function
            
            _originalFunctionAddress = originalFunctionInfo.MethodHandle.GetFunctionPointer();
            
            // Get the information about the target function
            
            var targetFunctionInfo = typeof(TTargetClass).GetMethod(targetFunction, BindingFlags.Instance | BindingFlags.Public);
            
            if (targetFunctionInfo is null)
            {
                throw new ArgumentException($"No function called {targetFunction} was found in the class {typeof(TTargetClass)}");
            }
            
            // Ensure the function is JIT compiled
            
            RuntimeHelpers.PrepareMethod(targetFunctionInfo.MethodHandle);
            
            // Get a pointer to the target function
            
            var targetFunctionAddress = targetFunctionInfo.MethodHandle.GetFunctionPointer();
            
            var isProcessX64 = Environment.Is64BitProcess;
            
            // Create shellcode to perform a function detour
            
            var shellcode = isProcessX64 ? Shellcode.JumpToFunctionX64(targetFunctionAddress) : Shellcode.JumpToFunctionX86(targetFunctionAddress);
            
            // Save the bytes of the original function
            
            _originalBytes = new byte[shellcode.Length];
                
            Marshal.Copy(_originalFunctionAddress, _originalBytes, 0, shellcode.Length);

            // Save the bytes used to detour the original function to the target function
            
            _detourBytes = shellcode;
        }
        
        public void AddDetour()
        {
            // Write the detour bytes to the start of the original function
            
            Tools.InternalWriteMemory(_originalFunctionAddress, _detourBytes);
            
            IsHookActive = true;
        }
        
        public void RemoveDetour()
        {
            // Write the original bytes to the start of the original function
            
            Tools.InternalWriteMemory(_originalFunctionAddress, _originalBytes);
            
            IsHookActive = false;
        }
    }
}