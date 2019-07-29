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
        public new static ElementwiseSequenceEqualityComparer<T> Default => new ElementwiseSequenceEqualityComparer<T>();

        // ReSharper disable once StaticMemberInGenericType
        private static readonly bool _typeHasDefinedOrder = !IsDictionaryType() && !IsSetType();
        
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
            
            return _typeHasDefinedOrder ? leftEnumerable.SequenceEqual(rightEnumerable) : ScrambledEquals(leftEnumerable, rightEnumerable);
        }

        public override int GetHashCode(T obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return 0;
            }

            unchecked
            {   
                // Special case for IDictionary, because KeyValuePair<> doesn't implement GetHashCode properly (no value semantics). We
                // use the runtime type here because there is no way to do this in a common way for the static dictionary base types 
                // IDictionary and IReadOnlyDictionary<,>.
                if (obj is IDictionary dict)
                {
                    var code = 17;

                    foreach (DictionaryEntry entry in dict)
                    {
                        code = code ^ (entry.Key?.GetHashCode() ?? 0) ^ (486187739 * (entry.Value?.GetHashCode() ?? 0));
                    }

                    return code;
                }

                if (_typeHasDefinedOrder)
                {
                    // Make sure that different order produces different hash codes
                    return obj.Cast<object>().Aggregate(17, (current, o) => (current * 486187739) ^ (o == null ? 0 : o.GetHashCode()));
                }

                // The sequence does not define order, so just XOR all elements
                return obj.Cast<object>().Aggregate(17, (current, o) => current ^ (o == null ? 0 : o.GetHashCode()));
            }
        }

        private static bool IsDictionaryType()
        {
            var type = typeof(T);

            // Typical dictionaries
            if (typeof(IDictionary).GetTypeInfo().IsAssignableFrom(type))
            {
                return true;
            }

            // Read-only dictionaries (these aren't subtypes of IDictionary, so a separate check is required)
            return type.GetTypeInfo().IsGenericType
                   && typeof(IReadOnlyDictionary<,>).GetTypeInfo().IsAssignableFrom(type.GetGenericTypeDefinition());
        }

        private static bool IsSetType()
        {
            var type = typeof(T).GetTypeInfo();
            
            // Sets don't have a non-generic base type
            return type.IsGenericType && typeof(ISet<>).GetTypeInfo().IsAssignableFrom(type.GetGenericTypeDefinition());
        }

        private static bool ScrambledEquals<TElem>(IEnumerable<TElem> list1, IEnumerable<TElem> list2)
        {
            var counters = new Dictionary<TElem, int>();
            foreach (var element in list1)
            {
                if (counters.ContainsKey(element))
                {
                    counters[element]++;
                }
                else
                {
                    counters.Add(element, 1);
                }
            }

            foreach (var element in list2)
            {
                if (counters.ContainsKey(element))
                {
                    counters[element]--;
                }
                else
                {
                    return false;
                }
            }

            return counters.Values.All(c => c == 0);
        }
    }
}
