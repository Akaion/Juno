using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Juno.Assembly;
using Juno.Memory;
using Juno.Tools;

namespace Juno
{
    /// <summary>
    /// Initialises an instance capable of detouring a function to call another function
    /// </summary>
    public class MethodDetour
    {
        private byte[] _detourBytes;
        
        private byte[] _originalBytes;
        
        private readonly RuntimeMethodHandle _originalMethodHandle;
        
        private readonly RuntimeMethodHandle _targetMethodHandle;

        /// <summary>
        /// Initialises an instance capable of detouring a function to call another function
        /// </summary>
        public MethodDetour(MethodInfo originalMethod, MethodInfo targetMethod)
        {
            if (originalMethod is null || targetMethod is null)
            {
                throw new ArgumentException("One or more the parameters was invalid");
            }
            
            // Ensure the method signatures match
            
            ValidationHandler.ValidateMethodSignature(originalMethod, targetMethod);

            // Get the original method handle
            
            _originalMethodHandle = originalMethod.MethodHandle;

            // Get the target method handle
            
            _targetMethodHandle = targetMethod.MethodHandle;

            PrepareDetour();
        }

        /// <summary>
        /// Initialises an instance capable of detouring a function to call another function
        /// </summary>
        protected MethodDetour(IReflect originalClass, string originalMethod, IReflect targetClass, string targetMethod)
        {
            if (originalClass is null || string.IsNullOrWhiteSpace(originalMethod) || targetClass is null || string.IsNullOrWhiteSpace(targetMethod))
            {
                throw new ArgumentException("One or more the parameters was invalid");
            }

            const BindingFlags bindingFlags = BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

            // Get the original method handle
            
            var originalMethodInfo = originalClass.GetMethod(originalMethod, bindingFlags) ?? throw new MissingMethodException($"Failed to find the method {originalMethod} in the class {nameof(originalClass)}");

            _originalMethodHandle = originalMethodInfo.MethodHandle;
            
            // Get the target method handle
            
            var targetMethodInfo = targetClass.GetMethod(targetMethod, bindingFlags) ?? throw new MissingMethodException($"Failed to find the method {targetMethod} in the class {nameof(targetClass)}");

            _targetMethodHandle = targetMethodInfo.MethodHandle;

            PrepareDetour();
        }

        /// <summary>
        /// Detours the original function to call the target function
        /// </summary>
        public void InitialiseDetour()
        {
            MemoryManager.WriteVirtualMemory(_originalMethodHandle.GetFunctionPointer(), _detourBytes);
        }

        /// <summary>
        /// Restores the original function to call the original function
        /// </summary>
        public void RemoveDetour()
        {
            MemoryManager.WriteVirtualMemory(_originalMethodHandle.GetFunctionPointer(), _originalBytes);
        }
        
        private void PrepareDetour()
        {
            // Ensure both methods are JIT compiled
            
            RuntimeHelpers.PrepareMethod(_originalMethodHandle);

            RuntimeHelpers.PrepareMethod(_targetMethodHandle);

            _detourBytes = Assembler.AssembleJump(_targetMethodHandle.GetFunctionPointer());
            
            // Save the bytes of the original method
            
            _originalBytes = new byte[_detourBytes.Length];
            
            Marshal.Copy(_originalMethodHandle.GetFunctionPointer(), _originalBytes, 0, _detourBytes.Length);
        }
    }

    /// <summary>
    /// Initialises an instance capable of detouring a function to call another function
    /// </summary>
    public class MethodDetour<TOriginalClass, TTargetClass> : MethodDetour where TOriginalClass : class where TTargetClass : class
    {
        /// <summary>
        /// Initialises an instance capable of detouring a function to call another function
        /// </summary>
        public MethodDetour(string originalMethod, string targetMethod) : base(typeof(TOriginalClass), originalMethod, typeof(TTargetClass), targetMethod) {}
    }
}