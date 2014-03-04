// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FinalList.cs" company="Dani Michel">
//   Dani Michel 2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Belt.Equatable
{
    using System;

    public abstract class ValueBasedEquatable<TSelf, TValue> : IEquatable<ValueBasedEquatable<TSelf, TValue>>
    {
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
            return Equals(EquatableValue, other.EquatableValue);
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
            return ReferenceEquals(EquatableValue, null) ? 0 : EquatableValue.GetHashCode();
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
