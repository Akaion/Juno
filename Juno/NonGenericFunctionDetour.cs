using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Juno.Etc;
using Juno.Interfaces;
using Juno.Services;

namespace Juno
{
    public class NonGenericFunctionDetour : IDetour
    {
        private readonly byte[] _detourBytes;
        
        public bool IsHookActive;
        
        private readonly byte[] _originalBytes;
        
        private readonly IntPtr _originalFunctionAddress;
        
        public NonGenericFunctionDetour(IReflect originalClassType, IReflect targetClassType, string originalFunctionName, string targetFunctionName)
        {
            // Ensure the operating system is valid
            
            ValidateOperatingSystem.Validate();
            
            // Ensure the arguments passed in are valid
            
            if (string.IsNullOrWhiteSpace(originalFunctionName) || string.IsNullOrWhiteSpace(targetFunctionName))
            {
                throw new ArgumentException("One or more of the arguments provided was invalid");
            }
            
            // Get the information about the original function
            
            var originalFunctionInfo = originalClassType.GetMethod(originalFunctionName, BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            
            if (originalFunctionInfo is null)
            {
                throw new ArgumentException($"No function called {originalFunctionName} was found in the class {originalClassType}");
            }
            
            // Ensure the function is JIT compiled
            
            RuntimeHelpers.PrepareMethod(originalFunctionInfo.MethodHandle);
            
            // Get a pointer to the original function
            
            _originalFunctionAddress = originalFunctionInfo.MethodHandle.GetFunctionPointer();
            
            // Get the information about the target function
            
            var targetFunctionInfo = targetClassType.GetMethod(targetFunctionName, BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            
            if (targetFunctionInfo is null)
            {
                throw new ArgumentException($"No function called {targetFunctionName} was found in the class {targetClassType}");
            }
            
            // Ensure the function is JIT compiled
            
            RuntimeHelpers.PrepareMethod(targetFunctionInfo.MethodHandle);
            
            // Get a pointer to the target function
            
            var targetFunctionAddress = targetFunctionInfo.MethodHandle.GetFunctionPointer();
            
            // Ensure the method signatures of the original and target function match
            
            Tools.ValidateMethodSignature(originalFunctionInfo, targetFunctionInfo);
            
            // Create shellcode to perform a function detour
            
            var shellcode = Environment.Is64BitProcess ? Shellcode.JumpToFunctionX64(targetFunctionAddress) : Shellcode.JumpToFunctionX86(targetFunctionAddress);
            
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