using System;
using System.Linq;
using System.Reflection;

namespace Juno.Tools
{
    internal static class ValidationHandler
    {
        internal static void ValidateMethodSignature(MethodInfo originalMethod, MethodInfo targetMethod)
        {
            // Ensure the parameters of both methods match

            var originalMethodParameters = originalMethod.GetParameters();

            var targetMethodParameters = targetMethod.GetParameters();
            
            foreach (var (originalParameters, targetParameters) in originalMethodParameters.Zip(targetMethodParameters, (originalParameters, targetParameters) => (originalParameters, targetParameters)))
            {
                if (originalParameters.ParameterType != targetParameters.ParameterType)
                {
                    throw new ArgumentException("The parameters of the methods did not match");
                }
            }
            
            // Ensure the return type of both methods match
            
            var originalMethodType = originalMethod.ReturnType;

            var targetMethodType = targetMethod.ReturnType;

            if (originalMethodType != targetMethodType)
            {
                throw new ArgumentException("The return type of the methods did not match");
            }
        }
    }
}