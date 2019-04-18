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
        private const BindingFlags MethodBindingFlags = BindingFlags.FlattenHierarchy | 
            BindingFlags.Instance | 
            BindingFlags.NonPublic | 
            BindingFlags.Public | 
            BindingFlags.Static;

        private byte[] _detourBytes;
        
        private byte[] _originalBytes;
        
        private IntPtr _originalFunctionAddress;

        public FunctionDetour(MethodInfo originalFunctionInfo, MethodInfo targetFunctionInfo)
        {
            if (originalFunctionInfo == null)
            {
                throw new ArgumentNullException($"The parameter '{nameof(originalFunctionInfo)}' can't be null!");
            }

            if (targetFunctionInfo == null)
            {
                throw new ArgumentNullException($"The parameter '{nameof(targetFunctionInfo)}' can't be null!");
            }

            if (targetFunctionInfo.MethodImplementationFlags != MethodImplAttributes.NoInlining)
            {
                throw new InvalidOperationException($"The function {targetFunctionInfo.Name} must be decorated with the NoInlining attribute.");
            }

            InitialiseDetour(originalFunctionInfo, targetFunctionInfo);
        }

        public FunctionDetour(IReflect originalClassType, string originalFunctionName, IReflect targetClassType, string targetFunctionName)
        {
            // Ensure the arguments passed in are valid

            if (string.IsNullOrWhiteSpace(originalFunctionName))
            {
                throw new ArgumentException($"The parameter '{nameof(originalFunctionName)}' can't be null / empty or whitespace!");
            }

            if (string.IsNullOrWhiteSpace(targetFunctionName))
            {
                throw new ArgumentException($"The parameter '{nameof(targetFunctionName)}' can't be null / empty or whitespace!");
            }

            if (originalClassType == null)
            {
                throw new ArgumentNullException($"The parameter '{nameof(originalClassType)}' can't be null!");
            }

            if (targetClassType == null)
            {
                throw new ArgumentNullException($"The parameter '{nameof(targetClassType)}' can't be null!");
            }

            // Get the information about the original function

            var originalFunctionInfo = originalClassType.GetMethod(originalFunctionName, MethodBindingFlags);

            if (originalFunctionInfo == null)
            {
                throw new InvalidOperationException($"Could not find function '{originalFunctionName}' in class {originalClassType}!");
            }

            // Get the information about the target function

            var targetFunctionInfo = targetClassType.GetMethod(targetFunctionName, MethodBindingFlags);

            if (targetFunctionInfo == null)
            {
                throw new InvalidOperationException($"Could not find function '{targetFunctionName}' in class {targetClassType}!");
            }

            if (targetFunctionInfo.MethodImplementationFlags != MethodImplAttributes.NoInlining)
            {
                throw new InvalidOperationException($"The function {targetFunctionName} must be decorated with the NoInlining attribute.");
            }

            InitialiseDetour(originalFunctionInfo, targetFunctionInfo);
        }

        private void InitialiseDetour(MethodInfo originalFunctionInfo, MethodInfo targetFunctionInfo)
        {
            // Ensure the operating system is valid

            ValidateOperatingSystem.Validate();

            // Ensure the functions are JIT compiled

            RuntimeHelpers.PrepareMethod(originalFunctionInfo.MethodHandle);

            RuntimeHelpers.PrepareMethod(targetFunctionInfo.MethodHandle);

            // Get a pointer to the original function

            _originalFunctionAddress = originalFunctionInfo.MethodHandle.GetFunctionPointer();

            // Get a pointer to the target function

            var targetFunctionAddress = targetFunctionInfo.MethodHandle.GetFunctionPointer();

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
        }
        
        public void RemoveDetour()
        {
            // Write the original bytes to the start of the original function
            
            Tools.InternalWriteMemory(_originalFunctionAddress, _originalBytes);
        }
    }
}