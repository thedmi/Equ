namespace Equ
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    // TODO Remove before stable version 
    [Obsolete]
    public class EquatableSequenceWrapper<T> : IEquatable<EquatableSequenceWrapper<T>>
    {
        private readonly IEnumerable<T> _enumerable;

        public EquatableSequenceWrapper(IEnumerable<T> enumerable)
        {
            _enumerable = enumerable;
        }

        public bool Equals(EquatableSequenceWrapper<T> other)
        {
            return _enumerable.SequenceEqual(other._enumerable);
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
            return Equals((EquatableSequenceWrapper<T>)obj);
        }

        public override int GetHashCode()
        {
            return _enumerable.Aggregate(0, (current, item) => current ^ item.GetHashCode());
        }

        public override string ToString()
        {
            return "[ "
                   + _enumerable.Aggregate("", (accu, item) => accu + item.ToString() + ", ")
                       .TrimEnd(new[] { ' ', ',' }) + " ]";
        }
    }
}