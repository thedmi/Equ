namespace Equ
{
    using System;

    public interface IPropertywiseEquatable<TSelf> : IEquatable<TSelf>, IPropertywiseEquatable { }

    public interface IPropertywiseEquatable { }
}
