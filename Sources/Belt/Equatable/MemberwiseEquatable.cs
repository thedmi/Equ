namespace Belt.Equatable
{
    using System;

    public abstract class MemberwiseEquatable<T> : IEquatable<T>
        where T : class
    {
        private static readonly MemberwiseEqualityComparer<T> _equalityComparer = new MemberwiseEqualityComparer<T>();

        public bool Equals(T other)
        {
            return _equalityComparer.Equals(this as T, other);
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
            return Equals((T)obj);
        }

        public override int GetHashCode()
        {
            return _equalityComparer.GetHashCode(this as T);
        }

        public static bool operator ==(MemberwiseEquatable<T> left, MemberwiseEquatable<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MemberwiseEquatable<T> left, MemberwiseEquatable<T> right)
        {
            return !Equals(left, right);
        }
    }
}
