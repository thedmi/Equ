namespace Equ
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    public static class ReflectionUtils
    {
        private static readonly Regex _autoPropertyBackingFieldRegex = new Regex("^<([a-zA-Z][a-zA-Z0-9]*)>k__BackingField$");

        /// <summary>
        /// Returns the associated auto property PropertyInfo if <paramref name="memberInfo"/> is a compiler-generated backing field.
        /// </summary>
        public static PropertyInfo GetPropertyForBackingField(MemberInfo memberInfo)
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
        
        public static bool IsSequenceType(Type type)
        {
            return typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(type) && type != typeof(string);
        }
    }
}