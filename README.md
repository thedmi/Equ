Equ
====

Equ is a .NET library that provides fast, convention-based equality operations for your types. This is primarily useful for custom value types (in the sense of the [value object pattern](http://en.wikipedia.org/wiki/Value_object), not in the sense of `struct`).

Members participating in equality operations are resolved through reflection, the resulting equality operations are then stored as compiled expression using LINQ expression trees. This makes the runtime performance as fast as with a manual implementation.

Grab the NuGet [here](https://www.nuget.org/packages/Equ/).


Usage
----------

### Simple Scenarios

The easiest way to leverage Equ is to derive your class from `MemberwiseEquatable<TSelf>` where `TSelf` is the deriving class itself. `MemberwiseEquatable<TSelf>` uses field-based equality, so your class gets `Equals()` and `GetHashCode()` implementations that consider all fields.

#### Example

The following example shows a very simple value object called `Address` that consists of two string members.

```csharp
using Equ;

class Address : MemberwiseEquatable<Address>
{
    private readonly string _street;
    private readonly string _city;

    public Address(string street, string city)
    {
        _street = street;
        _city = city;
    }

    public string Street { get { return _street; } }
    public string City { get { return _city; } }
}
```

Note that C# 6 read-only auto properties have compiler-generated backing fields as well, so there following code does exactly the same:


```csharp
using Equ;

class Address : MemberwiseEquatable<Address>
{
    public Address(string street, string city)
    {
        Street = street;
        City = city;
    }

    public string Street { get; }
    public string City { get; }
}
```

With this value object, the following expression is true, because `MemberwiseEquatable<Address>` provides an overload for the `==` operator that eventually *compares all private fields* of `Address`.

```csharp
new Address("Baker Street", "London") == new Address("Baker Street", "London") // true
```


### Customizable Equality Comparer

For most scenarios, the simple usage should be fine. But if you need more control or do not want to inherit from `MemberwiseEquatable<TSelf>`, just implement `IEquatable<T>` and delegate `Equals()` and `GetHashCode()` to an instance of `MemberwiseEqualityComparer<T>`. `MemberwiseEqualityComparer<T>` offers static instances through `ByFields` and `ByProperties`.

For very advanced scenarios, you can even create a completely customized comparer by using `MemberwiseEqualityComparer<T>.Create(EqualityFunctionGenerator)`.

#### Example

The same example as above, only this time we don't inherit from `MemberwiseEquatable`.

```csharp
using Equ;

class Address : IEquatable<Address>
{
    // Make sure the comparer is static, so that the equality operations are only generated once
    private static readonly MemberwiseEqualityComparer<Address> _comparer =
        MemberwiseEqualityComparer<Address>.ByFields;

    public Address(string street, string city)
    {
        Street = street;
        City = city;
    }

    public string Street { get; }
    public string City { get; }

    public bool Equals(Address other)
    {
        return _comparer.Equals(this, other);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Address);
    }

    public override int GetHashCode()
    {
        return _comparer.GetHashCode(this);
    }
}
```

### Compositional Integrity

At the core of Equ lives the `EqualityFunctionGenerator`, which is responsible for generating `Equals()` and `GetHashCode()` expressions given a type and a set of `MemberInfo` instances. The generator maintains compositional integrity in the sense of value objects, i.e. it compares objects value by value. To fully leverage this concept, the generator follows a set of rules when using members in an equality operation:

- For reference-typed members, a call to their `Equals()` or `GetHashCode()` methods is generated
- For value-typed members an equals operation (`a == b`) is generated, or the `GetHashCode()` of the boxed type is called
- For sequence-typed members, a call to an appropriate `ElementwiseSequenceEqualityComparer<T>` is generated

The `ElementwiseSequenceEqualityComparer<T>` is basically just a wrapper around `Enumerable.SequenceEqual()` with additional null checks.
