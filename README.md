wrapandcast
===========

WrapAndCast - cast a raw object to an interface

This is small library that helps casting raw objects to interfaces.
For example:

You have this interface:

```C#
public interface TestInterface
{
	string StringProp { get; }

	void TestMethod();
}
```

...and this class:

```C#
public class TestClass
{
	public string StringProp { get; private set; }

	public void TestMethod()
	{
		StringProp = "TestMethod2";
	}
}
```

The class implements the interface but we can't use it where the TestInterface is expected.

With this small library you can do this:

```C#
var rawObj = new TestClass();
var casted = rawObj.As<TestInterface>();
```

or

```C#
var casted = CastIt.To<TestInterface>(rawObj);
```

or

```C#
var casted = CastIt.To(typeof(TestInterface), rawObj);
```
