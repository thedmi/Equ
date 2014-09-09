namespace Equ
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An equality comparer for enumerable types that compares the enumerables element by element. Basically,
    /// this is a wrapper around <see cref="Enumerable.SequenceEqual{TSource}(System.Collections.Generic.IEnumerable{TSource},System.Collections.Generic.IEnumerable{TSource})"/>
    /// with additional null checking.
    /// </summary>
    /// <typeparam name="T">The type of the enumerable, i.e. a type implementing <see cref="IEnumerable"/></typeparam>
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
