using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Juno.Etc;
using Juno.Interfaces;
using Juno.Services;

namespace Juno
{
    public class FunctionDetour : IDetour
    {
        private byte[] _detourBytes;
        
        private byte[] _originalBytes;
        
        private IntPtr _originalFunctionAddress;

        /// <summary>
        /// All a method to be detoured by passing in methodinfo, rather than name, so that
        /// when multiple methods exist with the same name, we dont need to worry about that
        /// </summary>
        /// <param name="originalFunctionInfo">method info pointing to the function we want to replace</param>
        /// <param name="targetFunctionInfo">method info pointing to the function to replace with</param>
        public FunctionDetour(MethodInfo originalFunctionInfo, MethodInfo targetFunctionInfo)
        {
            this.InitialiseDetour(originalFunctionInfo, targetFunctionInfo);
        }

        public FunctionDetour(IReflect originalClassType, string originalFunctionName, IReflect targetClassType, string targetFunctionName)
        {
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

            // Get the information about the target function

            var targetFunctionInfo = targetClassType.GetMethod(targetFunctionName, BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

            if (targetFunctionInfo is null)
            {
                throw new ArgumentException($"No function called {targetFunctionName} was found in the class {targetClassType}");
            }

            this.InitialiseDetour(originalFunctionInfo, targetFunctionInfo);
        }

        private void InitialiseDetour(MethodInfo originalFunctionInfo, MethodInfo targetFunctionInfo)
        {
            // Ensure the operating system is valid

            ValidateOperatingSystem.Validate();

            // Get the information about the original function

            if (originalFunctionInfo is null)
            {
                throw new ArgumentException("originalFunctionInfo is null");
            }

            // Ensure the function is JIT compiled

            RuntimeHelpers.PrepareMethod(originalFunctionInfo.MethodHandle);

            // Get a pointer to the original function

            _originalFunctionAddress = originalFunctionInfo.MethodHandle.GetFunctionPointer();

            // Get the information about the target function

            if (targetFunctionInfo is null)
            {
                throw new ArgumentException("targetFunctionInfo is null");
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
        }
        
        public void RemoveDetour()
        {
            // Write the original bytes to the start of the original function
            
            Tools.InternalWriteMemory(_originalFunctionAddress, _originalBytes);
        }
    }
}