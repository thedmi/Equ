namespace Equ
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Provides an implementation of <see cref="IEqualityComparer{T}"/> that performs memberwise
    /// equality comparison of objects of type T. Use the <see cref="ByFields"/> or <see cref="ByProperties"/>
    /// static instances to get one of the two comparison strategies that are supported by default. Note that both
    /// honor <see cref="MemberwiseEqualityIgnoreAttribute"/>.
    /// 
    /// For more advanced scenarios, use the <see cref="Custom"/> creator and pass a <see cref="EqualityFunctionGenerator"/>
    /// that matches your requirements.
    /// </summary>
    public class MemberwiseEqualityComparer<T> : IEqualityComparer<T>
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly Regex _autoPropertyBackingFieldRegex = new Regex("^<([a-zA-Z][a-zA-Z0-9]*)>k__BackingField$");

        private readonly Func<object, object, bool> _equalsFunc;

        private readonly Func<object, int> _getHashCodeFunc;

        private static readonly Lazy<MemberwiseEqualityComparer<T>> _fieldsComparer =
            new Lazy<MemberwiseEqualityComparer<T>>(
                () =>
                    new MemberwiseEqualityComparer<T>(
                        new EqualityFunctionGenerator(
                            typeof(T),
                            AllFieldsExceptIgnored,
                            t => new List<PropertyInfo>())));

        private static readonly Lazy<MemberwiseEqualityComparer<T>> _propertiesComparer =
            new Lazy<MemberwiseEqualityComparer<T>>(
                () =>
                    new MemberwiseEqualityComparer<T>(
                        new EqualityFunctionGenerator(
                            typeof(T),
                            t => new List<FieldInfo>(),
                            AllPropertiesExceptIgnored)));

        public static MemberwiseEqualityComparer<T> ByFields => _fieldsComparer.Value;

        public static MemberwiseEqualityComparer<T> ByProperties => _propertiesComparer.Value;

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
            List<FieldInfo> fieldInfo = t.GetTypeInfo().GetFields(AllInstanceMembers).Where(IsNotMarkedAsIgnore).OrderBy(f => f.Name).ToList();
            fieldInfo.Sort((i1, i2) => i1.Name.CompareTo(i2.Name));

            return fieldInfo;
        }

        private static IEnumerable<PropertyInfo> AllPropertiesExceptIgnored(Type t)
        {
            List<PropertyInfo> propertyInfo = t.GetTypeInfo().GetProperties(AllInstanceMembers)
                                                             .Where(p => IsNotMarkedAsIgnore(p) && IsNotIndexed(p))
                                                             .OrderBy(p => p.Name).ToList();
            propertyInfo.Sort((i1, i2) => i1.Name.CompareTo(i2.Name));

            return propertyInfo;
        }

        private static BindingFlags AllInstanceMembers
        {
            get { return BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic; }
        }

        private static bool IsNotMarkedAsIgnore(MemberInfo memberInfo)
        {
            var isSelfIgnored = memberInfo.GetCustomAttributes(typeof(MemberwiseEqualityIgnoreAttribute), true).Any();

            var propertyInfo = GetPropertyForBackingField(memberInfo);
            var isPropertyIgnored = propertyInfo != null && propertyInfo.GetCustomAttributes(typeof(MemberwiseEqualityIgnoreAttribute), true).Any();

            return !isSelfIgnored && !isPropertyIgnored;
        }

        private static bool IsNotIndexed(PropertyInfo propertyInfo)
        {
            var indexParamaters = propertyInfo.GetIndexParameters();

            return indexParamaters.Count() == 0;
        }

        private static PropertyInfo GetPropertyForBackingField(MemberInfo memberInfo)
        {
            if (memberInfo is FieldInfo && _autoPropertyBackingFieldRegex.IsMatch(memberInfo.Name))
            {
                var match = _autoPropertyBackingFieldRegex.Match(memberInfo.Name);
                if (match.Success && match.Groups.Count >= 2 && match.Groups[1].Captures.Count >= 1)
                {
                    var propertyName = match.Groups[1].Captures[0].Value;
                    return memberInfo.DeclaringType.GetTypeInfo().GetProperties().SingleOrDefault(p => p.Name == propertyName);
                }
            }

            return null;
        }

        /// <summary>
        /// This method delegates to the generated equality function. Note that first a reference check 
        /// on <paramref name="x"/> and <paramref name="y"/> (reference equality and null-check) is performed.
        /// </summary>
        /// <returns>True if <paramref name="x"/> and <paramref name="y"/> are memberwise equal.</returns>
        public bool Equals(T x, T y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }
            if (ReferenceEquals(null, x) || ReferenceEquals(null, y))
            {
                return false;
            }
            return _equalsFunc(x, y);
        }

        /// <summary>
        /// This method delegates to the generated hash code function. If <paramref name="obj"/> is null,
        /// 0 is returned.
        /// </summary>
        /// <returns>The hash code for <paramref name="obj"/></returns>
        public int GetHashCode(T obj)
        {
            return ReferenceEquals(null, obj) ? 0 : _getHashCodeFunc(obj);
        }
    }
}
