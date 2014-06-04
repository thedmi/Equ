namespace Belt.Equatable
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

        public override bool Equals(T x, T y)
        {
            var ex = (IEnumerable<object>)x;
            var ey = (IEnumerable<object>)y;

            return ex.SequenceEqual(ey);
        }

        public override int GetHashCode(T obj)
        {
            var enumerable = (IEnumerable<object>)obj;

            return enumerable == null ? 0 : enumerable.Aggregate(0, (current, o) => current ^ o.GetHashCode());
        }
    }
}
