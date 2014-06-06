namespace Belt.Equatable
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Belt.FinalList;

    /// <summary>
    ///  Provides an implementation of EqualityComparer that performs memberwise
    ///  equality comparison of objects.
    /// </summary>
    public class MemberwiseEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<object, object, bool> _equalsFunc;

        private readonly Func<object, int> _getHashCodeFunc;

        private static readonly Lazy<MemberwiseEqualityComparer<T>> _fieldsComparer =
            new Lazy<MemberwiseEqualityComparer<T>>(
                () =>
                    new MemberwiseEqualityComparer<T>(
                        new EqualityFunctionGenerator(
                            typeof(T),
                            AllFieldsExceptIgnored,
                            t => FinalList.Empty<PropertyInfo>())));

        private static readonly Lazy<MemberwiseEqualityComparer<T>> _propertiesComparer =
            new Lazy<MemberwiseEqualityComparer<T>>(
                () =>
                    new MemberwiseEqualityComparer<T>(
                        new EqualityFunctionGenerator(
                            typeof(T),
                            t => FinalList.Empty<FieldInfo>(),
                            AllPropertiesExceptIgnored)));

        public static MemberwiseEqualityComparer<T> ByFields { get { return _fieldsComparer.Value; } }

        public static MemberwiseEqualityComparer<T> ByProperties { get { return _propertiesComparer.Value; } }

        public static MemberwiseEqualityComparer<T> Custom(EqualityFunctionGenerator equalityFunctionGenerator)
        {
            return new MemberwiseEqualityComparer<T>(equalityFunctionGenerator);
        } 

        private MemberwiseEqualityComparer(EqualityFunctionGenerator equalityFunctionGenerator)
        {
            _equalsFunc = equalityFunctionGenerator.MakeEqualsMethod();
            _getHashCodeFunc = equalityFunctionGenerator.MakeGetHashCodeMethod();
        }

        private static IEnumerable<FieldInfo> AllFieldsExceptIgnored(Type t)
        {
            return
                t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(fi => fi.GetCustomAttributes(typeof(MemberwiseEqualityIgnoreAttribute), true).Length == 0);
        }

        private static IEnumerable<PropertyInfo> AllPropertiesExceptIgnored(Type t)
        {
            return
                t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(fi => fi.GetCustomAttributes(typeof(MemberwiseEqualityIgnoreAttribute), true).Length == 0);
        }

        public bool Equals(T x, T y)
        {
            return _equalsFunc(x, y);
        }

        public int GetHashCode(T obj)
        {
            return _getHashCodeFunc(obj);
        }
    }
}
