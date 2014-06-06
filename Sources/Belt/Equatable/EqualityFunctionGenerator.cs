namespace Belt.Equatable
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Fasterflect;

    public static class EqualityFunctionGenerator
    {
        private static readonly MethodInfo _objectEqualsMethod = typeof(object).GetMethod("Equals", BindingFlags.Static | BindingFlags.Public);
        
        public static Func<object, int> MakeGetHashCodeMethod(Type type)
        {
            var objRaw = Expression.Parameter(typeof(object), "obj");

            // cast to the subclass type
            var objParam = Expression.Convert(objRaw, type);

            // compound XOr expression
            var getHashCodeExprs = GetIncludedMembers(type).Select(p => MakeGetHashCodeExpression(p, p.FieldType, objParam));
            var xorChainExpr = getHashCodeExprs.Aggregate((Expression)Expression.Constant(29), LinkHashCodeExpression);
            
            return Expression.Lambda<Func<object, int>>(xorChainExpr, objRaw).Compile();
        }

        public static Func<object, object, bool> MakeEqualsMethod(Type type)
        {
            var leftRaw = Expression.Parameter(typeof(object), "left");
            var rightRaw = Expression.Parameter(typeof(object), "right");

            // cast to the subclass type
            var leftParam = Expression.Convert(leftRaw, type);
            var rightParam = Expression.Convert(rightRaw, type);

            // compound AND expression using short-circuit evaluation
            var equalsExprs = GetIncludedMembers(type).Select(p => MakeEqualsExpression(p, p.FieldType, leftParam, rightParam));
            var andChainExpr = equalsExprs.Aggregate((Expression)Expression.Constant(true), Expression.AndAlso);

            // call Object.Equals if second parameter doesn't match type
            var objectEqualsExpr = Expression.Equal(leftRaw, rightRaw);
            var useTypedEqualsExpression = Expression.Condition(
                Expression.TypeIs(rightRaw, type),
                andChainExpr,
                objectEqualsExpr);

            return Expression.Lambda<Func<object, object, bool>>(useTypedEqualsExpression, leftRaw, rightRaw).Compile();
        }

        private static IEnumerable<FieldInfo> GetIncludedMembers(Type type)
        {
            return
                type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(fi => fi.GetCustomAttributes(typeof(MemberwiseEqualityIgnoreAttribute), true).Length == 0);
        }

        private static Expression LinkHashCodeExpression(Expression left, Expression right)
        {
            var leftMultiplied = Expression.Multiply(left, Expression.Constant(486187739));
            return Expression.ExclusiveOr(leftMultiplied, right);
        }

        private static Expression MakeEqualsExpression(MemberInfo member, Type memberType, Expression left, Expression right)
        {
            var leftMemberExpr = Expression.MakeMemberAccess(left, member);
            var rightMemberExpr = Expression.MakeMemberAccess(right, member);

            if (memberType.IsValueType)
            {
                return MakeValueTypeEqualExpression(leftMemberExpr, rightMemberExpr);
            }
            if (IsSequenceType(memberType))
            {
                return MakeSequenceTypeEqualExpression(leftMemberExpr, rightMemberExpr, memberType);
            }
            return MakeReferenceTypeEqualExpression(leftMemberExpr, rightMemberExpr);
        }

        private static Expression MakeValueTypeEqualExpression(Expression left, Expression right)
        {
            return Expression.Equal(left, right);
        }

        private static Expression MakeSequenceTypeEqualExpression(Expression left, Expression right, Type enumerableType)
        {
            return MakeCallOnSequenceEqualityComparerExpression("Equals", enumerableType, left, right);
        }

        private static Expression MakeReferenceTypeEqualExpression(Expression left, Expression right)
        {
            return Expression.Call(_objectEqualsMethod, left, right);
        }

        private static Expression MakeGetHashCodeExpression(MemberInfo member, Type memberType, UnaryExpression obj)
        {
            var memberAccessExpr = Expression.MakeMemberAccess(obj, member);

            var getHashCodeExpr = IsSequenceType(memberType)
                ? MakeCallOnSequenceEqualityComparerExpression("GetHashCode", memberType, memberAccessExpr)
                : Expression.Call(memberAccessExpr, "GetHashCode", Type.EmptyTypes);

            return Expression.Condition(
                Expression.ReferenceEqual(Expression.Constant(null), Expression.Convert(memberAccessExpr, typeof(object))), // If member is null
                Expression.Constant(0), // Return 0
                getHashCodeExpr); // Return the actual getHashCode call
        }

        private static Expression MakeCallOnSequenceEqualityComparerExpression(string methodName, Type enumerableType, params Expression[] parameterExpressions)
        {
            var comparerType = typeof(ElementwiseSequenceEqualityComparer<>).MakeGenericType(enumerableType);
            var comparerInstance = comparerType.GetProperty("Default", BindingFlags.Static | BindingFlags.Public).GetValue(null);
            var comparerExpr = Expression.Constant(comparerInstance);

            return Expression.Call(comparerExpr, methodName, Type.EmptyTypes, parameterExpressions);
        }

        private static bool IsSequenceType(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);
        }
    }
}