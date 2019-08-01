# Juno

[![Build status](https://ci.appveyor.com/api/projects/status/81cs42eqbvnfcumx?svg=true)](https://ci.appveyor.com/project/Akaion/juno)

A Windows managed method detouring library that supports both x86 and x64 detours.

----

### Installation

* Download and install Juno using [NuGet](https://www.nuget.org/packages/Juno)

----

### Usage

The example below describes a basic implementation of the library

```csharp
using Juno;

public class TestClass1
{
    public void TestMethod1()
    {
        Console.WriteLine("Original method called");
    }
}

public class TestClass2
{
    public void TestMethod2()
    {
        Console.WriteLine("Detoured method called");
    }
}

var detour = new MethodDetour<TestClass1, TestClass2>("TestMethod1", "TestMethod2");

var testClass = new TestClass1();

// Calls the original method as expected

testClass.TestMethod1();

// Add the method detour

detour.InitialiseDetour();

// Calls the target (detoured) method (TestClass2.TestMethod2)

testClass.TestMethod1();

// Remove the method detour

detour.RemoveDetour();

// Calls the original method

testClass.TestMethod1;

```

----

### Overloads

In lieu of providing a set of classes and method names, you can provide a set of `MethodInfo` datatypes.

```csharp
var detour = new MethodDetour(MethodInfo1, MethodInfo2);
```

----

### Caveats

* Both original and detoured classes must have the same layout of instance variables if you wish to access them.

* Both original and detoured methods cannot be inlined be the compiler.

* Detoured methods must match the method signature of the original method (same parameters & return type.)

----

### Contributing

Pull requests are welcome. 

For large changes, please open an issue first to discuss what you would like to contribute.
