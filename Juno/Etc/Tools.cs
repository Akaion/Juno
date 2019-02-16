using System;
using System.Linq;
using System.Reflection;
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

        internal static void ValidateMethodSignature(MethodInfo originalFunction, MethodInfo targetFunction)
        {
            // Ensure the parameters of the target function match the parameters of the original function
            
            var originalParameters = originalFunction.GetParameters();
            
            var targetParameters = targetFunction.GetParameters();
            
            foreach (var (originalParameterInfo, targetParameterInfo) in originalParameters.Zip(targetParameters, (originalParameterInfo, targetParameterInfo) => (originalParameterInfo, targetParameterInfo)))
            {
                if (originalParameterInfo.ParameterType != targetParameterInfo.ParameterType)
                {
                    throw new ArgumentException("The parameters of the target function did not match the parameters of the original function");
                }
            }
            
            // Ensure the return type of the target function matches the return type of the original function
            
            var originalReturnType = originalFunction.ReturnType;
            
            var targetReturnType = targetFunction.ReturnType;
            
            if (originalReturnType != targetReturnType)
            {
                throw new ArgumentException("The return type of the target function did not match the return type of the original function");
            }
        }
    }
}