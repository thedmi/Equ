Equ
====

Equ is a .NET library that provides fast, convention-based equality operations for your types. This is primarily useful for custom value types (in the sense of the value type pattern, not in the sense of `struct`). 

Members participating in equality operations are resolved through reflection, the resulting equality operations are then stored as compiled expression using LINQ expression trees. 

Grab the NuGet [here](https://www.nuget.org/packages/Equ/).


Usage
----------

### Simple Scenarios

The easiest way to leverage Equ is to derive your class from `MemberwiseEquatable<TSelf>` where `TSelf` is the deriving class itself. `MemberwiseEquatable<TSelf>` uses field-based equality, so your class gets `Equals()` and `GetHashCode()` implementations that consider all fields. 


### Customizable Equality Comparer

If you need more control or do not want to inherit from `MemberwiseEquatable<TSelf>`, just implement `IEquatable<T>` and delegate `Equals()` and `GetHashCode()` to an instance of `MemberwiseEqualityComparer<T>`. `MemberwiseEqualityComparer<T>` offers static instances through `ByFields` and `ByProperties`. Or you can even create a completely customized comparer by using `MemberwiseEqualityComparer<T>.Create(EqualityFunctionGenerator)`.


### Compositional Integrity

At the core of Equ lives the `EqualityFunctionGenerator`, which is responsible for generating `Equals()` and `GetHashCode()` expressions given a type and a set of `MemberInfo` instances. The generator maintains compositional integrity in the sense of value objects, i.e. it compares objects value by value. To fully leverage this concept, the generator follows a set of rules when using members in an equality operation:

- For reference-typed members, a call to their `Equals()` or `GetHashCode()` methods is generated
- For value-typed members an equals operation (`a == b`) is generated, or the `GetHashCode()` of the boxed type is called
- For sequence-typed members, a call to an appropriate `ElementwiseSequenceEqualityComparer<T>` is generated

The `ElementwiseSequenceEqualityComparer<T>` is basically just a wrapper around `Enumerable.SequenceEqual()` with additional null checks.


