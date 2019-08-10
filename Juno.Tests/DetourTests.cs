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
    
    public class DetourTests
    {
        private readonly MethodDetour _methodDetour;

        private readonly TestClass1 _testClass;

        private readonly int _testVariable1;

        private readonly int _testVariable2;

        public DetourTests()
        {
            _methodDetour = new MethodDetour<TestClass1, TestClass2>("TestMethod", "TestMethod");
            
            _testClass = new TestClass1();

            _testVariable1 = 5;

            _testVariable2 = 10;   
        }

        [Fact]
        public void TestInitialiseDetour()
        {
            _methodDetour.InitialiseDetour();
            
            Assert.Equal(50, _testClass.TestMethod(_testVariable1, _testVariable2));
            
            _methodDetour.RemoveDetour();
        }

        [Fact]
        public void TestRemoveDetour()
        {
            _methodDetour.InitialiseDetour();
            
            _methodDetour.RemoveDetour();
            
            Assert.Equal(15, _testClass.TestMethod(_testVariable1, _testVariable2));
        }
    }
}