namespace Equ
{
    using System;

    public interface IMemberwiseEquatable<TSelf> : IEquatable<TSelf>, IMemberwiseEquatable { }

    public interface IMemberwiseEquatable { }
}