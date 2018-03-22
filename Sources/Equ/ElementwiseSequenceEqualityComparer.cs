namespace Equ
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

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
            
            if (typeof(IDictionary).GetTypeInfo().IsAssignableFrom(typeof(T)))
            {
                IDictionary leftDict = (IDictionary)left;
                IDictionary rightDict = (IDictionary)right;

                if(leftDict.Keys.Count != rightDict.Keys.Count)
                {
                    return false;
                }

                foreach(var key in leftDict.Keys)
                {
                    if (!rightDict.Contains(key) || !rightDict[key].Equals(leftDict[key]))
                    {
                        return false;
                    }
                }

                return true;
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

            if (typeof(IDictionary).GetTypeInfo().IsAssignableFrom(typeof(T)))
            {
                IDictionary dict = (IDictionary)obj;

                var value = dict.Keys.Cast<object>().ToDictionary(k => k, k => dict[k])
                                .Select(p => new KeyValuePair<int, int>(p.Key.GetHashCode(), p.Value?.GetHashCode() ?? 0))
                                .OrderBy(p => p.Key).ThenBy(p => p.Value);
                unchecked
                {
                    return value.Aggregate(17, (current, p) => (((current * 486187739) ^ p.Key) * 486187739) ^ p.Value);
                }
            }

            unchecked
            {
                return obj.Cast<object>().Aggregate(17, (current, o) => (current * 486187739) ^ (o == null ? 0 : o.GetHashCode()));
            }
        }
    }
}
