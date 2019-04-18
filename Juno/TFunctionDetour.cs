namespace Juno
{
    public class FunctionDetour<TOriginal, TDetour> : FunctionDetour
        where TOriginal : class
        where TDetour : class
    {
        public FunctionDetour(string originalFunctionName, string targetFunctionName) : 
            base(typeof(TOriginal), originalFunctionName, typeof(TDetour), targetFunctionName)
        {
        }
    }
}
