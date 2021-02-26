namespace Equ
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// This class is able to produce compiled Equals() and GetHashCode() functions.
    /// </summary>
    public class EqualityFunctionGenerator
    {
        private static readonly MethodInfo _objectEqualsMethod = new Func<object, object, bool>(Equals).GetMethodInfo();

        private readonly Type _type;

        private readonly Func<Type, IEnumerable<FieldInfo>> _fieldSelector;
        private readonly Func<Type, IEnumerable<PropertyInfo>> _propertySelector;
        private readonly string _memberwiseEqualityComparerProperty;
        private readonly string _elementwiseSequenceEqualityComparerProperty;

        /// <summary>
        /// Creates a generator for type <paramref name="type"/>. The generated functions will consider all fields
        /// and properties of <paramref name="type"/> that are returned by <paramref name="fieldSelector"/> and 
        /// <paramref name="propertySelector"/>, respectively.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fieldSelector"></param>
        /// <param name="propertySelector"></param>
        public EqualityFunctionGenerator(Type type, Func<Type, IEnumerable<FieldInfo>> fieldSelector, Func<Type, IEnumerable<PropertyInfo>> propertySelector, MemberwiseEqualityMode mode)
        {
            _type = type;
            _fieldSelector = fieldSelector;
            _propertySelector = propertySelector;

            _memberwiseEqualityComparerProperty = mode switch
            {
                MemberwiseEqualityMode.ByFields => nameof(MemberwiseEqualityComparer<object>.ByFields),
                MemberwiseEqualityMode.ByFieldsRecursive => nameof(MemberwiseEqualityComparer<object>.ByFieldsRecursive),
                MemberwiseEqualityMode.ByProperties => nameof(MemberwiseEqualityComparer<object>.ByProperties),
                MemberwiseEqualityMode.ByPropertiesRecursive => nameof(MemberwiseEqualityComparer<object>.ByPropertiesRecursive),
                _ => string.Empty
            };

            _elementwiseSequenceEqualityComparerProperty = mode switch
            {
                MemberwiseEqualityMode.ByFieldsRecursive => nameof(ElementwiseSequenceEqualityComparer<IEnumerable<object>>.ByFieldsRecursive),
                MemberwiseEqualityMode.ByPropertiesRecursive => nameof(ElementwiseSequenceEqualityComparer<IEnumerable<object>>.ByPropertiesRecursive),
                _ => nameof(ElementwiseSequenceEqualityComparer<IEnumerable<object>>.Default)
            };
        }

        /// <summary>
        /// Generates a GetHashCode() function. The generated function should be cached at class-level scope, so that
        /// the generation only takes place once per type. This can be achieved e.g. by storing the function in a static
        /// readonly field.
        /// </summary>
        public Func<object, int> MakeGetHashCodeMethod()
        {
            var objRaw = Expression.Parameter(typeof(object), "obj");

            // cast to the concrete type
            var objParam = Expression.Convert(objRaw, _type);

            // compound XOR expression
            var getHashCodeExprs = GetIncludedMembers(_type).Select(mi => MakeGetHashCodeExpression(mi, objParam, _elementwiseSequenceEqualityComparerProperty));
            var xorChainExpr = getHashCodeExprs.Aggregate((Expression)Expression.Constant(29), LinkHashCodeExpression);
            
            return Expression.Lambda<Func<object, int>>(xorChainExpr, objRaw).Compile();
        }

        /// <summary>
        /// Generates a Equals() function. The generated function should be cached at class-level scope, so that
        /// the generation only takes place once per type. This can be achieved e.g. by storing the function in a static
        /// readonly field.
        /// </summary>
        public Func<object, object, bool> MakeEqualsMethod()
        {
            var leftRaw = Expression.Parameter(typeof(object), "left");
            var rightRaw = Expression.Parameter(typeof(object), "right");

            // cast to the concrete type
            var leftParam = Expression.Convert(leftRaw, _type);
            var rightParam = Expression.Convert(rightRaw, _type);

            // AND expression using short-circuit evaluation
            var equalsExprs = GetIncludedMembers(_type).Select(mi => MakeEqualsExpression(mi, leftParam, rightParam, _memberwiseEqualityComparerProperty, _elementwiseSequenceEqualityComparerProperty));
            var andChainExpr = equalsExprs.Aggregate((Expression)Expression.Constant(true), Expression.AndAlso);

            // call Object.Equals if second parameter doesn't match type
            var objectEqualsExpr = Expression.Equal(leftRaw, rightRaw);
            var useTypedEqualsExpression = Expression.Condition(
                Expression.TypeIs(rightRaw, _type),
                andChainExpr,
                objectEqualsExpr);

            return Expression.Lambda<Func<object, object, bool>>(useTypedEqualsExpression, leftRaw, rightRaw).Compile();
        }

        private IEnumerable<MemberInfo> GetIncludedMembers(Type type)
        {
            return _fieldSelector(type).Cast<MemberInfo>().Concat(_propertySelector(type));
        }

        private static Expression LinkHashCodeExpression(Expression left, Expression right)
        {
            var leftMultiplied = Expression.Multiply(left, Expression.Constant(486187739));
            return Expression.ExclusiveOr(leftMultiplied, right);
        }

        private static Expression MakeEqualsExpression(MemberInfo member, Expression left, Expression right, string memberwiseEqualityComparerProperty, string elementwiseSequenceEqualityComparerProperty)
        {
            var leftMemberExpr = Expression.MakeMemberAccess(left, member);
            var rightMemberExpr = Expression.MakeMemberAccess(right, member);

            var memberType = leftMemberExpr.Type;

            if (leftMemberExpr.Type.GetTypeInfo().IsValueType)
            {
                var boxedLeftMemberExpr = Expression.Convert(leftMemberExpr, typeof(object));
                var boxedRightMemberExpr = Expression.Convert(rightMemberExpr, typeof(object));
                return MakeReferenceTypeEqualExpression(boxedLeftMemberExpr, boxedRightMemberExpr, memberType, string.Empty);
            }

            return ReflectionUtils.IsSequenceType(memberType)
                ? MakeSequenceTypeEqualExpression(leftMemberExpr, rightMemberExpr, memberType, elementwiseSequenceEqualityComparerProperty)
                : MakeReferenceTypeEqualExpression(leftMemberExpr, rightMemberExpr, memberType, memberwiseEqualityComparerProperty);
        }

        private static Expression MakeSequenceTypeEqualExpression(Expression left, Expression right, Type enumerableType, string elementwiseSequenceEqualityComparerProperty)
        {
            return MakeCallOnSequenceEqualityComparerExpression("Equals", enumerableType, elementwiseSequenceEqualityComparerProperty, left, right);
        }

        private static Expression MakeReferenceTypeEqualExpression(Expression left, Expression right, Type memberType, string memberwiseEqualityComparerProperty)
        {
            if (nameof(MemberwiseEqualityComparer<object>.ByFieldsRecursive).Equals(memberwiseEqualityComparerProperty)
             || nameof(MemberwiseEqualityComparer<object>.ByPropertiesRecursive).Equals(memberwiseEqualityComparerProperty))
            {
                return MakeCallOnMemberwiseEqualityComparerExpression("Equals", memberType, memberwiseEqualityComparerProperty, left, right);
            }
            else
            {
                return Expression.Call(_objectEqualsMethod, left, right);
            }
        }

        private static Expression MakeGetHashCodeExpression(MemberInfo member, Expression obj, string elementwiseSequenceEqualityComparerProperty)
        {
            var memberAccessExpr = Expression.MakeMemberAccess(obj, member);
            var memberAccessAsObjExpr = Expression.Convert(memberAccessExpr, typeof(object));

            var memberType = memberAccessExpr.Type;

            var getHashCodeExpr = ReflectionUtils.IsSequenceType(memberType)
                ? MakeCallOnSequenceEqualityComparerExpression("GetHashCode", memberType, elementwiseSequenceEqualityComparerProperty, memberAccessExpr)
                : Expression.Call(memberAccessAsObjExpr, "GetHashCode", Type.EmptyTypes);

            return Expression.Condition(
                Expression.ReferenceEqual(Expression.Constant(null), memberAccessAsObjExpr), // If member is null
                Expression.Constant(0), // Return 0
                getHashCodeExpr); // Return the actual getHashCode call
        }

        private static Expression MakeCallOnSequenceEqualityComparerExpression(string methodName, Type enumerableType, string elementwiseSequenceEqualityComparerProperty, params Expression[] parameterExpressions)
        {
            var comparerType = typeof(ElementwiseSequenceEqualityComparer<>).MakeGenericType(enumerableType);
            var comparerInstance = comparerType.GetTypeInfo().GetProperty(elementwiseSequenceEqualityComparerProperty, BindingFlags.Static | BindingFlags.Public).GetValue(null);
            var comparerExpr = Expression.Constant(comparerInstance);

            return Expression.Call(comparerExpr, methodName, Type.EmptyTypes, parameterExpressions);
        }

        private static Expression MakeCallOnMemberwiseEqualityComparerExpression(string methodName, Type enumerableType, string memberwiseEqualityComparerProperty, params Expression[] parameterExpressions)
        {
            var comparerType = typeof(MemberwiseEqualityComparer<>).MakeGenericType(enumerableType);
            var comparerInstance = comparerType.GetTypeInfo().GetProperty(memberwiseEqualityComparerProperty, BindingFlags.Static | BindingFlags.Public).GetValue(null);
            var comparerExpr = Expression.Constant(comparerInstance);

            return Expression.Call(comparerExpr, methodName, Type.EmptyTypes, parameterExpressions);
        }
    }
}