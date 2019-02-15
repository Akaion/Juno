using Juno.Interfaces;

namespace Juno
{
    public class GenericFunctionDetour<TOriginalClass, TTargetClass> : IDetour
    {
        public bool IsHookActive;
        
        private readonly NonGenericFunctionDetour _functionDetour;
        
        public GenericFunctionDetour(string originalFunctionName, string targetFunctionName)
        {   
            _functionDetour = new NonGenericFunctionDetour(typeof(TOriginalClass), typeof(TTargetClass), originalFunctionName, targetFunctionName);
        }
        
        public void AddDetour()
        {
            // Add a function detour
            
            _functionDetour.AddDetour();
            
            IsHookActive = true;
        }
        
        public void RemoveDetour()
        {
            // Remove a function detour
            
            _functionDetour.RemoveDetour();
            
            IsHookActive = false;
        }
    }
}