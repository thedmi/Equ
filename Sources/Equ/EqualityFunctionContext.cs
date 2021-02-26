using System;
using System.Collections.Generic;

namespace Equ
{
    public class EqualityFunctionContext
    {
        private readonly Dictionary<Type, MemberwiseEqualityComparer> _equalityComparers;

        public EqualityFunctionContext(MemberwiseEqualityMode mode)
        {
            _equalityComparers = new Dictionary<Type, MemberwiseEqualityComparer>();

            Mode = mode;

            MemberwiseEqualityComparerProperty = mode switch
            {
                MemberwiseEqualityMode.ByFields => nameof(MemberwiseEqualityComparer<object>.ByFields),
                MemberwiseEqualityMode.ByFieldsRecursive => nameof(MemberwiseEqualityComparer<object>.ByFieldsRecursive),
                MemberwiseEqualityMode.ByProperties => nameof(MemberwiseEqualityComparer<object>.ByProperties),
                MemberwiseEqualityMode.ByPropertiesRecursive => nameof(MemberwiseEqualityComparer<object>.ByPropertiesRecursive),
                _ => string.Empty
            };

            ElementwiseSequenceEqualityComparerProperty = mode switch
            {
                MemberwiseEqualityMode.ByFieldsRecursive => nameof(ElementwiseSequenceEqualityComparer<IEnumerable<object>>.ByFieldsRecursive),
                MemberwiseEqualityMode.ByPropertiesRecursive => nameof(ElementwiseSequenceEqualityComparer<IEnumerable<object>>.ByPropertiesRecursive),
                _ => nameof(ElementwiseSequenceEqualityComparer<IEnumerable<object>>.Default)
            };
        }

        public bool TryGetEqualityComparer(Type type, out MemberwiseEqualityComparer equalityComparer)
        {
            return _equalityComparers.TryGetValue(type, out equalityComparer);
        }

        public void Add<T>(MemberwiseEqualityComparer<T> equalityComparer)
        {
            _equalityComparers.Add(typeof(T), equalityComparer);
        }

        public MemberwiseEqualityMode Mode { get; }
        public bool IsRecursive => Mode == MemberwiseEqualityMode.ByFieldsRecursive || Mode == MemberwiseEqualityMode.ByPropertiesRecursive;
        public string MemberwiseEqualityComparerProperty { get; }
        public string ElementwiseSequenceEqualityComparerProperty { get; }
    }
}
