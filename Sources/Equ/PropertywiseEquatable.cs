namespace Equ
{
    using System;

    public class PropertywiseEquatable<TSelf> : IPropertywiseEquatable<TSelf>
    {
        static PropertywiseEquatable()
        {
            if (!typeof(PropertywiseEquatable<TSelf>).IsAssignableFrom(typeof(TSelf)))
            {
                throw new ArgumentException("The type argument TSelf must be a subclass of PropertywiseEquatable<TSelf>");
            }
        } 

        private static readonly MemberwiseEqualityComparer<TSelf> _equalityComparer = MemberwiseEqualityComparer<TSelf>.ByProperties;

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

        public static bool operator ==(PropertywiseEquatable<TSelf> left, PropertywiseEquatable<TSelf> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PropertywiseEquatable<TSelf> left, PropertywiseEquatable<TSelf> right)
        {
            return !Equals(left, right);
        }
    }
}
