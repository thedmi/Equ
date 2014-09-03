namespace Equ
{
    using System;

    [Obsolete("Use MemberwiseEquatable instead. It has the same features, but a simpler interface.")]
    public abstract class ValueBasedEquatable<TSelf, TValue> : IEquatable<ValueBasedEquatable<TSelf, TValue>>, IValueBasedEquatable<TValue>
    {
        private static readonly MemberwiseEqualityComparer<Tuple<TValue>> _equalityComparer = MemberwiseEqualityComparer<Tuple<TValue>>.ByFields;

        protected abstract TValue EquatableValue { get; }

        public bool Equals(ValueBasedEquatable<TSelf, TValue> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return _equalityComparer.Equals(Tuple.Create(EquatableValue), Tuple.Create(other.EquatableValue));
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
            return Equals((ValueBasedEquatable<TSelf, TValue>)obj);
        }

        public override int GetHashCode()
        {
            return ReferenceEquals(EquatableValue, null) ? 0 : _equalityComparer.GetHashCode(Tuple.Create(EquatableValue));
        }

        public static bool operator ==(ValueBasedEquatable<TSelf, TValue> id1, ValueBasedEquatable<TSelf, TValue> id2)
        {
            return Equals(id1, id2);
        }

        public static bool operator !=(ValueBasedEquatable<TSelf, TValue> id1, ValueBasedEquatable<TSelf, TValue> id2)
        {
            return !(id1 == id2);
        }

        public override string ToString()
        {
            return EquatableValue.ToString();
        }
    }
}
