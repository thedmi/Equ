Belt
====

This is a very lightweight utility library that increases expressiveness of your programs. It provides concepts for immutable lists, maybes (optional values) and a very simple guard library.

Also available as [nuget package](https://www.nuget.org/packages/Belt/).


Final List
----------

The `IFinalList<T>` concept denotes a non-lazy, immutable list that is covariant in its item type `T`.

The motivation for `IFinalList` was that there is no concept of a list in .NET that carries all of these properties.

- `IReadOnlyList<T>` is covariant in T and eagerly loaded, but is read-only, not immutable 
- `System.Collections.Immutable.IImmutableList<T>` is immutable and carries eager loading semantics, but is not covariant in T
- `IEnumerable<T>` is covariant in T, but uses lazy-loading and also read-only semantics

`System.Collections.Immutable.IImmutableList<T>` would have been the perfect candidate for an immutable list concept. But unfortunately, someone decided to provide the immutable `Add()`, `Remove()` and similar methods not only on the implementation `ImmutableList<T>`, but also on the interface, effectively making the item type invariant. 

To differentiate the immutable list of this library from `System.Collections.Immutable.IImmutableList<T>`, I call it a *Final List*. 

For a discussion about immutable vs. read-only semantics see [the BCL team blog](http://blogs.msdn.com/b/bclteam/archive/2012/12/18/preview-of-immutable-collections-released-on-nuget.aspx).


Maybe 
-----

The `IMaybe<T>` interface denotes a covariant maybe type that can be used to represent optional values or relations.

In my programs, `null` is never, ever a valid value and always considered a bug. I employ this strict rule for the sake of readability, because

- I never have to check actual parameters for null, since calling the method with null would be an error of the caller.
- There is no implicit, hidden, incomprehensible notion of a nonexisting value that would be a valid method parameter to mean "nothing" or "empty" or the like.

If we cannot use null for optional things like 0..1 relations, we need something else. Enter `IMaybe<T>`. You will never ask yourself again "do I need a null check here?".


Lazy 
-----

The `ILazy<T>` interface denotes a lazily evaluated, covariant type wrapper for T. It uses `System.Lazy` internally, so this library just wraps it to achieve covariance.


ValueBasedEquatable
--------------------

Implementing equality operations is a common task that introduces a lot of noise into the respective classes. Most of the time, the goal is the same: Implementing the equality members based on one or more values.

The abstract `ValueBasedEquatable<TSelf, TValue>` class is a reusable, null-safe and correct equality 
implementation based on `TValue`. The class implements `IEquatable` and overrides `Equals()`, `GetHashCode()`, `ToString()` and the equality operators.

Equality based on more than one value can easily be implemented by using a `Tuple` of the desired type as `TValue`.


Guards
------

This library provides the simplest conceivable form of guards. They exist in the form of precondition, postcondition, and generic guard functions that take a boolean and an optional violation message. 


