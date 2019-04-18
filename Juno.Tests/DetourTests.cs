using System;
using System.Reflection;
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

        public int TestMethodWithoutAttribute()
        {
            return 1;
        }
    }
    
    public class DetourTests
    {
        private readonly FunctionDetour _functionDetour;
        
        private readonly TestClass1 _testClass;
        
        private readonly int _testVariable1;
        
        private readonly int _testVariable2;
        
        public DetourTests()
        {
            // Initialize a function detour instance
            
            _functionDetour = new FunctionDetour(typeof(TestClass1), "TestMethod", typeof(TestClass2), "TestMethod");
            
            // Initialize a test class
            
            _testClass = new TestClass1();
            
            // Initialise test variables
            
            _testVariable1 = 5;
            
            _testVariable2 = 10;
        }

        [Fact]
        public void FunctionDetour_MethodInfoCtor_WhenOriginalFunctionInfoIsNull_ThrowsException()
        {
            var targetFunctionInfo = typeof(TestClass2).GetMethod(nameof(TestClass2.TestMethod));

            var ex = Assert.Throws<ArgumentNullException>(() => new FunctionDetour(null, targetFunctionInfo));

            Assert.Contains($"The parameter 'originalFunctionInfo' can't be null!", ex.Message);
        }

        [Fact]
        public void FunctionDetour_MethodInfoCtor_WhenTargetFunctionInfoIsNull_ThrowsException()
        {
            var originalFunctionInfo = typeof(TestClass1).GetMethod(nameof(TestClass1.TestMethod));

            var ex = Assert.Throws<ArgumentNullException>(() => new FunctionDetour(originalFunctionInfo, null));

            Assert.Contains($"The parameter 'targetFunctionInfo' can't be null!", ex.Message);            
        }

        [Fact]
        public void FunctionDetour_MethodInfoCtor_WhenTargetFunctionInfoDoesNotHaveNoInliningAttribute_ThrowsException()
        {
            var originalFunctionInfo = typeof(TestClass1).GetMethod(nameof(TestClass1.TestMethod));
            var targetFunctionInfo = typeof(TestClass2).GetMethod(nameof(TestClass2.TestMethodWithoutAttribute));

            var ex = Assert.Throws<InvalidOperationException>(() => new FunctionDetour(originalFunctionInfo, targetFunctionInfo));

            Assert.Equal($"The function {nameof(TestClass2.TestMethodWithoutAttribute)} must be decorated with the NoInlining attribute.", ex.Message);
        }

        [Fact]
        public void FunctionDetour_WhenOriginalFunctionNameIsNull_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new FunctionDetour<TestClass1, TestClass2>(null, nameof(TestClass2.TestMethod)));

            Assert.Equal("The parameter 'originalFunctionName' can't be null / empty or whitespace!", ex.Message);
        }

        [Fact]
        public void FunctionDetour_WhenOriginalFunctionNameIsEmpty_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new FunctionDetour<TestClass1, TestClass2>(string.Empty, nameof(TestClass2.TestMethod)));

            Assert.Equal("The parameter 'originalFunctionName' can't be null / empty or whitespace!", ex.Message);
        }

        [Fact]
        public void FunctionDetour_WhenOriginalFunctionNameIsWhiteSpace_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new FunctionDetour<TestClass1, TestClass2>(" ", nameof(TestClass2.TestMethod)));

            Assert.Equal("The parameter 'originalFunctionName' can't be null / empty or whitespace!", ex.Message);
        }

        [Fact]
        public void FunctionDetour_WhenTargetFunctionNameIsNull_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new FunctionDetour<TestClass1, TestClass2>(nameof(TestClass1.TestMethod), null));

            Assert.Equal("The parameter 'targetFunctionName' can't be null / empty or whitespace!", ex.Message);
        }

        [Fact]
        public void FunctionDetour_WhenTargetFunctionNameIsEmpty_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new FunctionDetour<TestClass1, TestClass2>(nameof(TestClass1.TestMethod), string.Empty));

            Assert.Equal("The parameter 'targetFunctionName' can't be null / empty or whitespace!", ex.Message);
        }

        [Fact]
        public void FunctionDetour_WhenTargetFunctionNameIsWhiteSpace_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new FunctionDetour<TestClass1, TestClass2>(nameof(TestClass1.TestMethod), " "));

            Assert.Equal("The parameter 'targetFunctionName' can't be null / empty or whitespace!", ex.Message);
        }

        [Fact]
        public void FunctionDetour_WhenOriginalClassTypeIsNull_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new FunctionDetour(null, nameof(TestClass1.TestMethod), typeof(TestClass2), nameof(TestClass2.TestMethod)));

            Assert.Contains("The parameter 'originalClassType' can't be null!", ex.Message);
        }

        [Fact]
        public void FunctionDetour_WhenTargetClassTypeIsNull_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new FunctionDetour(typeof(TestClass1), nameof(TestClass1.TestMethod), null, nameof(TestClass2.TestMethod)));

            Assert.Contains("The parameter 'targetClassType' can't be null!", ex.Message);
        }

        [Fact]
        public void FunctionDetour_WhenOriginalFunctionInfoIsNull_ThrowsException()
        {
            var methodName = "NonExistingMethod";

            var ex = Assert.Throws<InvalidOperationException>(() =>
                new FunctionDetour<TestClass1, TestClass2>(methodName, nameof(TestClass2.TestMethod)));

            Assert.Equal($"Could not find function '{methodName}' in class {typeof(TestClass1)}!", ex.Message);
        }

        [Fact]
        public void FunctionDetour_WhenTargetFunctionInfoIsNull_ThrowsException()
        {
            var methodName = "NonExistingMethod";

            var ex = Assert.Throws<InvalidOperationException>(() =>
                new FunctionDetour<TestClass1, TestClass2>(nameof(TestClass1.TestMethod), methodName));

            Assert.Equal($"Could not find function '{methodName}' in class {typeof(TestClass2)}!", ex.Message);
        }

        [Fact]
        public void FunctionDetour_WhenTargetFunctionInfoDoesNotHaveNoInliningAttribute_ThrowsException()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
                new FunctionDetour<TestClass1, TestClass2>(nameof(TestClass1.TestMethod), nameof(TestClass2.TestMethodWithoutAttribute)));

            Assert.Equal($"The function {nameof(TestClass2.TestMethodWithoutAttribute)} must be decorated with the NoInlining attribute.", ex.Message);
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
        public void TestAddDetourWithMethodInfo()
        {
            var originalMethodInfo = typeof(TestClass1).GetMethod("TestMethod", BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);

            var targetMethodInfo = typeof(TestClass2).GetMethod("TestMethod", BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);

            // Initialise a function detour

            var functionDetour = new FunctionDetour(originalMethodInfo, targetMethodInfo);
            
            // Initialize a test class

            var testClass1 = new TestClass1();

            functionDetour.AddDetour();

            Assert.Equal(2, testClass1.TestMethod(1, 2));

            functionDetour.RemoveDetour();
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
 