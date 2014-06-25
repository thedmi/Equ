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

        public override bool Equals(T xs, T ys)
        {
            if (ReferenceEquals(xs, ys))
            {
                return true;
            }
            if (ReferenceEquals(null, xs))
            {
                return false;
            }
            if (ReferenceEquals(null, ys))
            {
                return false;
            }

            var ex = xs.Cast<object>();
            var ey = ys.Cast<object>();

            return ex.SequenceEqual(ey);
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
