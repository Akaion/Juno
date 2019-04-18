# Juno

[![Build status](https://ci.appveyor.com/api/projects/status/81cs42eqbvnfcumx?svg=true)](https://ci.appveyor.com/project/Akaion/juno)

A Windows managed function detouring library written in C# that supports both x86 and x64 detours.

----

### Features

* x86 and x64 detours

----

### Limitations

* Both detoured and original classes must have the same layout of class variables if you wish to access them
* Both detoured and orginal functions can not be inlined
* Detoured functions must match the signature of the original function i.e same parameters and return type

----

### Installation

* Download and install Juno using [NuGet](https://www.nuget.org/packages/Juno)

----

### Useage

#### Basic detour

```csharp
using Juno;

public class TestClass1
{
    public void TestMethod1()
    {
        Console.WriteLine("Original Function Called");
    }
}

public class TestClass2
{
    public void TestMethod2()
    {
        Console.WriteLine("Detoured Function Called");
    }
}

public class Program
{
    public static void Main()
    {
        var functionDetour = new FunctionDetour(typeof(TestClass1), "TestMethod1", typeof(TestClass2), "TestMethod2");
        
        // Initialize a test class
        
        var testClass = new TestClass1();
        
        // This calls the original function as expected
        
        testClass.TestMethod1(); 
        
        // Add the function detour
        
        functionDetour.AddDetour();
        
        // This calls the detoured function TestClass2.TestMethod2
        
        testClass.TestMethod1();
        
        // Remove the detour
        
        functionDetour.RemoveDetour();
        
        // This calls the original function again
        
        testClass.TestMethod1();
    }
}
```

#### Accessing parameters of detoured functions

```csharp
using Juno;

public class TestClass1
{
    public void TestMethod1(int a, int b)
    {
        Console.WriteLine(a + b);
    }
}

public class TestClass2
{
    public void TestMethod2(int a, int b)
    {
        Console.WriteLine(a * b);
    }
}

public class Program
{
    public static void Main()
    {
        var functionDetour = new FunctionDetour(typeof(TestClass1), "TestMethod1", typeof(TestClass2), "TestMethod2");
        
        // Initialize a test class
        
        var testClass = new TestClass1();
        
        // This calls the original function as expected producing 5 to the console
        
        testClass.TestMethod1(2, 3); 
        
        // Add the function detour
        
        functionDetour.AddDetour();
        
        // This calls the detoured function TestClass2.TestMethod2 producing 6 to the console
        
        testClass.TestMethod1(2, 3);
        
        // Remove the detour
        
        functionDetour.RemoveDetour();
        
        // This calls the original function again producing 5 to the console
        
        testClass.TestMethod1();
    }
}
```

#### Accessing returns of detoured functions

```csharp
using Juno;

public class TestClass1
{
    public int TestMethod1()
    {
        return 5;
    }
}

public class TestClass2
{
    public void TestMethod2()
    {
        return 10;
    }
}

public class Program
{
    public static void Main()
    {
        var functionDetour = new FunctionDetour(typeof(TestClass1), "TestMethod1", typeof(TestClass2), "TestMethod2");
        
        // Initialize a test class
        
        var testClass = new TestClass1();
        
        // This calls the original function as expected returning 5
        
        var testValue1 = testClass.TestMethod1(); 
        
        // Add the function detour
        
        functionDetour.AddDetour();
        
        // This calls the detoured function TestClass2.TestMethod2 returning 10
        
        var testValue2 = testClass.TestMethod1();
        
        // Remove the detour
        
        functionDetour.RemoveDetour();
        
        // This calls the original function as expected returning 5
        
        var testValue3 = testClass.TestMethod1();
    }
}
```

#### Accessing original class variables from detoured functions

```csharp
using Juno;

public class TestClass1
{
    private int classVariable = 8;

    public void TestMethod1()
    {
        Console.WriteLine($"Class variable was {classVariable}");
    }
}

public class TestClass2
{
    private int classVariable = 0;

    public void TestMethod2()
    {
       Console.WriteLine($"Class variable was {classVariable}");
    }
}

public class Program
{
    public static void Main()
    {
        var functionDetour = new FunctionDetour(typeof(TestClass1), "TestMethod1", typeof(TestClass2), "TestMethod2");
        
        // Initialize a test class
        
        var testClass = new TestClass1();
        
        // This calls the original function as expected producing 8 to the console
        
        testClass.TestMethod1(); 
        
        // Add the function detour
        
        functionDetour.AddDetour();
        
        // This calls the detoured function TestClass2.TestMethod2 producing 0 to the console
        
        testClass.TestMethod1();
        
        // Remove the detour
        
        functionDetour.RemoveDetour();
        
        // This calls the original function as expected producing 8 to the console
        
        testClass.TestMethod1();
    }
}
```

----

### Contributing

Pull requests are welcome. 

For large changes, please open an issue first to discuss what you would like to add.

