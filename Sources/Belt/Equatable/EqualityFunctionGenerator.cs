namespace Belt.Equatable
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class EqualityFunctionGenerator
    {
        private static readonly MethodInfo _objectEqualsMethod = typeof(object).GetMethod("Equals", BindingFlags.Static | BindingFlags.Public);
        
        public static Func<object, int> MakeGetHashCodeMethod(Type type)
        {
            var objRaw = Expression.Parameter(typeof(object), "obj");

            // cast to the subclass type
            var objParam = Expression.Convert(objRaw, type);

            // compound XOr expression
            var getHashCodeExprs = GetIncludedMembers(type).Select(p => MakeGetHashCodeExpression(p, objParam));
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
            var equalsExprs = GetIncludedMembers(type).Select(p => MakeEqualsExpression(p, leftParam, rightParam));
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

        private static Expression MakeEqualsExpression(FieldInfo field, Expression left, Expression right)
        {
            var leftField = Expression.Field(left, field);
            var rightField = Expression.Field(right, field);

            var fieldType = field.FieldType;

            if (fieldType.IsValueType)
            {
                return MakeValueTypeEqualExpression(leftField, rightField);
            }
            if (IsSequenceType(fieldType))
            {
                return MakeSequenceTypeEqualExpression(leftField, rightField, fieldType);
            }
            return MakeReferenceTypeEqualExpression(leftField, rightField);
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

        private static Expression MakeGetHashCodeExpression(FieldInfo field, UnaryExpression obj)
        {
            var fieldExpr = Expression.Field(obj, field);

            var getHashCodeExpr = IsSequenceType(field.FieldType)
                ? MakeCallOnSequenceEqualityComparerExpression("GetHashCode", field.FieldType, fieldExpr)
                : Expression.Call(fieldExpr, "GetHashCode", Type.EmptyTypes);

            return Expression.Condition(
                Expression.ReferenceEqual(Expression.Constant(null), Expression.Convert(fieldExpr, typeof(object))), // If field is null
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