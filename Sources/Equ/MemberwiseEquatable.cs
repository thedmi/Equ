namespace Equ
{
    using System;
    using System.Reflection;

    /// <summary>
    /// By inheriting from this class, subclasses receive automatically generated <see cref="IEquatable{T}.Equals(T)"/>
    /// and <see cref="object.GetHashCode"/> implementations based on all fields that are not marked with the 
    /// <see cref="MemberwiseEqualityIgnoreAttribute"/>.
    /// 
    /// If you cannot inherit from a class or require customization of the equality comparison, implement <see cref="IEquatable{T}"/>
    /// and directly use a <see cref="MemberwiseEqualityComparer{T}"/>.
    /// 
    /// Note that the generated methods are cached for performance reasons.
    /// </summary>
    /// <typeparam name="TSelf">The concrete type that should be equatable. This is almost always the type that
    /// derives from <see cref="MemberwiseEquatable{TSelf}"/>, e.g. <code>class MyClass : MemberwiseEquatable&lt;MyClass&gt;</code>
    /// </typeparam>
    public abstract class MemberwiseEquatable<TSelf> : IMemberwiseEquatable<TSelf>
    {
        static MemberwiseEquatable()
        {
            if (!typeof(MemberwiseEquatable<TSelf>).GetTypeInfo().IsAssignableFrom(typeof(TSelf)))
            {
                throw new ArgumentException("The type argument TSelf must be a subclass of MemberwiseEquatable<TSelf>");
            }
        } 

        private static readonly MemberwiseEqualityComparer<TSelf> _equalityComparer = MemberwiseEqualityComparer<TSelf>.ByFields;

        public bool Equals(TSelf other)
        {
            return _equalityComparer.Equals((TSelf)(object)this, other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((TSelf)obj);
        }

        public override int GetHashCode()
        {
            return _equalityComparer.GetHashCode((TSelf)(object)this);
        }

        public static bool operator ==(MemberwiseEquatable<TSelf> left, MemberwiseEquatable<TSelf> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MemberwiseEquatable<TSelf> left, MemberwiseEquatable<TSelf> right)
        {
            return !Equals(left, right);
        }
    }
}
