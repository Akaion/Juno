using System.Runtime.CompilerServices;
using Xunit;

namespace Juno.Tests
{
    public class TestClass1
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public int TestMethod(int a, int b)
        {
            return a + b;
        }
    } 
    
    public class TestClass2
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public int TestMethod(int a, int b)
        {
            return a * b;
        }
    }
    
    public class Tests
    {
        private readonly FunctionDetour<TestClass1, TestClass2> _functionDetour;
        
        private readonly TestClass1 _testClass;
        
        private readonly int _testVariable1;
        
        private readonly int _testVariable2;
        
        public Tests()
        {
            // Initialize a function detour instance
            
            _functionDetour = new FunctionDetour<TestClass1, TestClass2>("TestMethod", "TestMethod");
            
            // Initialize a test class
            
            _testClass = new TestClass1();
            
            // Initialise test variables
            
            _testVariable1 = 5;
            
            _testVariable2 = 10;
        }
        
        [Fact]
        public void TestAddDetour()
        {
            // Add the detour
            
            _functionDetour.AddDetour();
            
            Assert.Equal(50, _testClass.TestMethod(_testVariable1, _testVariable2));
           
            // Remove the detour
            
            _functionDetour.RemoveDetour();
        }
        
        [Fact]
        public void TestRemoveDetour()
        {
            // Add the detour
            
            _functionDetour.AddDetour();
            
            // Remove the detour
            
            _functionDetour.RemoveDetour();
            
            Assert.Equal(15, _testClass.TestMethod(_testVariable1, _testVariable2));
        }
    }
}