using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Juno.Native.Enumerations;
using Juno.Native.PInvoke;
using Juno.Shared;
using Juno.Shared.Exceptions;

namespace Juno
{
    /// <summary>
    /// An instance capable of detouring a method to call another method
    /// </summary>
    public class MethodDetour
    {
        private byte[] _detourBytes;
        
        private byte[] _originalMethodBytes;

        private readonly RuntimeMethodHandle _originalMethodHandle;

        private readonly RuntimeMethodHandle _targetMethodHandle;

        /// <summary>
        /// An instance capable of detouring a method to call another method
        /// </summary>
        public MethodDetour(MethodInfo originalMethod, MethodInfo targetMethod)
        {
            if (originalMethod is null || targetMethod is null)
            {
                throw new ArgumentException("One or more the parameters was invalid");
            }
            
            ValidationHandler.ValidateMethodSignature(originalMethod, targetMethod);
            
            _originalMethodHandle = originalMethod.MethodHandle;
            
            _targetMethodHandle = targetMethod.MethodHandle;
            
            PrepareDetour();
        }
        
        /// <summary>
        /// An instance capable of detouring a method to call another method
        /// </summary>
        public MethodDetour(IReflect originalClass, string originalMethod, IReflect targetClass, string targetMethod)
        {
            if (originalClass is null || string.IsNullOrWhiteSpace(originalMethod) || targetClass is null || string.IsNullOrWhiteSpace(targetMethod))
            {
                throw new ArgumentException("One or more the parameters was invalid");
            }

            const BindingFlags bindingFlags = BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            
            var originalMethodInfo = originalClass.GetMethod(originalMethod, bindingFlags) ?? throw new MissingMethodException($"Failed to find the method {originalMethod} in the class {nameof(originalClass)}");

            _originalMethodHandle = originalMethodInfo.MethodHandle;
            
            var targetMethodInfo = targetClass.GetMethod(targetMethod, bindingFlags) ?? throw new MissingMethodException($"Failed to find the method {targetMethod} in the class {nameof(targetClass)}");

            _targetMethodHandle = targetMethodInfo.MethodHandle;

            PrepareDetour();
        }

        /// <summary>
        /// Detours the original method to call the target method
        /// </summary>
        public void InitialiseDetour()
        {
            var originalMethodAddress = _originalMethodHandle.GetFunctionPointer();
            
            // Adjust the protection of the virtual memory region to ensure it has write privileges

            if (!Kernel32.VirtualProtect(originalMethodAddress, _detourBytes.Length, MemoryProtectionType.ReadWrite, out var oldProtection))
            {
                throw new PInvokeException("Failed to call VirtualProtect");
            }
            
            Marshal.Copy(_detourBytes, 0, originalMethodAddress, _detourBytes.Length);
            
            // Restore the original protection of the virtual memory region

            if (!Kernel32.VirtualProtect(originalMethodAddress, _detourBytes.Length, oldProtection, out _))
            {
                throw new PInvokeException("Failed to call VirtualProtect");
            }
        }

        /// <summary>
        /// Restores the original method to call the original method
        /// </summary>
        public void RemoveDetour()
        {
            var originalMethodAddress = _originalMethodHandle.GetFunctionPointer();
            
            // Adjust the protection of the virtual memory region to ensure it has write privileges

            if (!Kernel32.VirtualProtect(originalMethodAddress, _originalMethodBytes.Length, MemoryProtectionType.ReadWrite, out var oldProtection))
            {
                throw new PInvokeException("Failed to call VirtualProtect");
            }
            
            Marshal.Copy(_originalMethodBytes, 0, originalMethodAddress, _originalMethodBytes.Length);
            
            // Restore the original protection of the virtual memory region

            if (!Kernel32.VirtualProtect(originalMethodAddress, _originalMethodBytes.Length, oldProtection, out _))
            {
                throw new PInvokeException("Failed to call VirtualProtect");
            }
        }

        private void PrepareDetour()
        {
            // Ensure both methods are JIT compiled
            
            RuntimeHelpers.PrepareMethod(_originalMethodHandle);

            RuntimeHelpers.PrepareMethod(_targetMethodHandle);

            // Construct the shellcode needed to detour the method
            
            var shellcode = new List<byte>();
            
            if (Environment.Is64BitProcess)
            {
                // mov rax, jumpAddress
                
                shellcode.AddRange(new byte[] {0x48, 0xB8});
                
                shellcode.AddRange(BitConverter.GetBytes((long) _targetMethodHandle.GetFunctionPointer()));
                
                // jmp rax
                
                shellcode.AddRange(new byte[] {0xFF, 0xE0});
            }

            else
            {
                // mov eax, jumpAddress
                
                shellcode.Add(0xB8);
                
                shellcode.AddRange(BitConverter.GetBytes((int) _targetMethodHandle.GetFunctionPointer()));
                
                // jmp eax
                
                shellcode.AddRange(new byte[] {0xFF, 0xE0});
            }

            _detourBytes = shellcode.ToArray();
            
            // Save the bytes of the original method
            
            _originalMethodBytes = new byte[_detourBytes.Length];
            
            Marshal.Copy(_originalMethodHandle.GetFunctionPointer(), _originalMethodBytes, 0, _detourBytes.Length);
        }
    }

    /// <summary>
    /// An instance capable of detouring a method to call another method
    /// </summary>
    public class MethodDetour<TOriginalClass, TTargetClass> : MethodDetour where TOriginalClass : class where TTargetClass : class
    {
        /// <summary>
        /// An instance capable of detouring a method to call another method
        /// </summary>
        public MethodDetour(string originalMethod, string targetMethod) : base(typeof(TOriginalClass), originalMethod, typeof(TTargetClass), targetMethod) { }
    }
}