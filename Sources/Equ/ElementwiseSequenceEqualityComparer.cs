namespace Equ
{
    using System;
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
        private static readonly Type EnumerableType = typeof(T)
            .GetTypeInfo()
            .GetInterfaces()
            .Where(type => type.GetTypeInfo().IsGenericType && typeof(IEnumerable<>).GetTypeInfo().IsAssignableFrom(type.GetGenericTypeDefinition()))
            .SelectMany(type => type.GetTypeInfo().GetGenericArguments())
            .SingleOrDefault();

        private static readonly MethodInfo SequenceEqualsMethodInfo = (EnumerableType == null) ? null : typeof(Enumerable)
            .GetTypeInfo()
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(methodInfo => methodInfo.Name.Equals(nameof(Enumerable.SequenceEqual)) && methodInfo.GetParameters().Length == 3)
            .Single()
            .MakeGenericMethod(EnumerableType);

        private static readonly MethodInfo ScrambledEqualsMethodInfo = (EnumerableType == null) ? null : typeof(ElementwiseSequenceEqualityComparer<T>)
            .GetTypeInfo()
            .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Where(methodInfo => methodInfo.Name.Equals(nameof(ElementwiseSequenceEqualityComparer<T>.ScrambledEquals)) && methodInfo.IsGenericMethod)
            .Single()
            .MakeGenericMethod(EnumerableType);

        // ReSharper disable once UnusedMember.Global
        public new static ElementwiseSequenceEqualityComparer<T> Default => new ElementwiseSequenceEqualityComparer<T>(MemberwiseEqualityMode.None);
        public static ElementwiseSequenceEqualityComparer<T> ByFieldsRecursive => new ElementwiseSequenceEqualityComparer<T>(MemberwiseEqualityMode.ByFieldsRecursive);
        public static ElementwiseSequenceEqualityComparer<T> ByPropertiesRecursive => new ElementwiseSequenceEqualityComparer<T>(MemberwiseEqualityMode.ByPropertiesRecursive);

        // ReSharper disable once StaticMemberInGenericType
        private static readonly bool _typeHasDefinedOrder = !IsDictionaryType() && !IsSetType();

        private bool _recursive;
        private Lazy<object> _memberwiseEqualityComparer;

        public ElementwiseSequenceEqualityComparer() : this(MemberwiseEqualityMode.None) { }

        internal ElementwiseSequenceEqualityComparer(MemberwiseEqualityMode mode)
        {
            _recursive = (mode == MemberwiseEqualityMode.ByFieldsRecursive || mode == MemberwiseEqualityMode.ByPropertiesRecursive);
            _memberwiseEqualityComparer = new Lazy<object>(() => CreateMemberwiseEqualityComparer(mode));
        }

        private object CreateMemberwiseEqualityComparer(MemberwiseEqualityMode mode)
        {
            var propertyName = mode switch
            {
                MemberwiseEqualityMode.ByFieldsRecursive => nameof(MemberwiseEqualityComparer<object>.ByFieldsRecursive),
                MemberwiseEqualityMode.ByPropertiesRecursive => nameof(MemberwiseEqualityComparer<object>.ByPropertiesRecursive),
                _ => nameof(MemberwiseEqualityComparer<object>.ByProperties)
            };

            return typeof(MemberwiseEqualityComparer<>)
                .GetTypeInfo()
                .MakeGenericType(EnumerableType)
                .GetTypeInfo()
                .GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public)
                .GetValue(null);
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

            return _typeHasDefinedOrder ? SequenceEqual(left, right) : ScrambledEquals(left, right);
        }

        private bool SequenceEqual(IEnumerable left, IEnumerable right)
        {
            if (_recursive && EnumerableType != null && !EnumerableType.GetTypeInfo().IsPrimitive)
            {
                return (bool)SequenceEqualsMethodInfo.Invoke(null, new [] { left, right, _memberwiseEqualityComparer.Value });
            }
            else
            {
                var leftEnumerable = left.Cast<object>();
                var rightEnumerable = right.Cast<object>();
                return leftEnumerable.SequenceEqual(rightEnumerable);
            }
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

        private bool ScrambledEquals(IEnumerable left, IEnumerable right)
        {
            if (_recursive && EnumerableType != null && !EnumerableType.GetTypeInfo().IsPrimitive)
            {
                return (bool)ScrambledEqualsMethodInfo.Invoke(null, new [] { left, right, _memberwiseEqualityComparer.Value });
            }
            else
            {
                var leftEnumerable = left.Cast<object>();
                var rightEnumerable = right.Cast<object>();

                return ScrambledEquals(leftEnumerable, rightEnumerable, EqualityComparer<object>.Default);
            }
        }

        private static bool ScrambledEquals<TElem>(IEnumerable<TElem> list1, IEnumerable<TElem> list2, IEqualityComparer<TElem> equalityComparer)
        {
            var counters = new Dictionary<TElem, int>(equalityComparer);
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
