namespace Belt.Equatable
{
    using System;

    public abstract class MemberwiseEquatable<TSelf> : IEquatable<TSelf>
        where TSelf : class
    {
        private static readonly MemberwiseEqualityComparer<TSelf> _equalityComparer = MemberwiseEqualityComparer<TSelf>.ByFields;

        public bool Equals(TSelf other)
        {
            return _equalityComparer.Equals(this as TSelf, other);
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
            return _equalityComparer.GetHashCode(this as TSelf);
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
