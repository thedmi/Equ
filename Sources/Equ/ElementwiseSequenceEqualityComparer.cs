namespace Equ
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class ElementwiseSequenceEqualityComparer<T> : EqualityComparer<T> where T : IEnumerable
    {
        // ReSharper disable once UnusedMember.Global
        public static new ElementwiseSequenceEqualityComparer<T> Default
        {
            get { return new ElementwiseSequenceEqualityComparer<T>(); }
        }

        public override bool Equals(T left, T right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            if (ReferenceEquals(null, left))
            {
                return false;
            }
            if (ReferenceEquals(null, right))
            {
                return false;
            }

            var leftEnumerable = left.Cast<object>();
            var rightEnumerable = right.Cast<object>();

            return leftEnumerable.SequenceEqual(rightEnumerable);
        }

        public override int GetHashCode(T obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return 0;
            }

            unchecked
            {
                return obj.Cast<object>().Aggregate(17, (current, o) => (current * 486187739) ^ o.GetHashCode());
            }
        }
    }
}
