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

        private static IEnumerable<FieldInfo> GetIncludedMembers(Type type)
        {
            return
                type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(fi => fi.GetCustomAttributes(typeof(MemberwiseEqualityIgnoreAttribute), true).Length == 0);
        }

        public static Func<object, int> MakeGetHashCodeMethod(Type type)
        {
            var objRaw = Expression.Parameter(typeof(object), "obj");

            // cast to the subclass type
            var objParam = Expression.Convert(objRaw, type);

            // compound XOr expression
            var getHashCodeExprs = GetIncludedMembers(type).Select(p => MakeGetHashCodeExpression(p, objParam));
            var xorChainExpr = getHashCodeExprs.Aggregate((Expression)Expression.Constant(0), Expression.ExclusiveOr);
            
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

        private static Expression MakeEqualsExpression(FieldInfo field, Expression left, Expression right)
        {
            var leftField = Expression.Field(left, field);
            var rightField = Expression.Field(right, field);

            var fieldType = field.FieldType;

            if (fieldType.IsValueType)
            {
                return MakeValueTypeEqualExpression(leftField, rightField);
            }
            if (fieldType.IsAssignableFrom(typeof(IEnumerable)) && fieldType != typeof(string))
            {
                return MakeSequenceTypeEqualExpression(leftField, rightField);
            }
            return MakeReferenceTypeEqualExpression(leftField, rightField);
        }

        private static Expression MakeValueTypeEqualExpression(Expression left, Expression right)
        {
            return Expression.Equal(left, right);
        }

        private static Expression MakeSequenceTypeEqualExpression(Expression left, Expression right)
        {
            // TODO Implement by element sequence comparison
            return MakeReferenceTypeEqualExpression(left, right);
        }

        private static Expression MakeReferenceTypeEqualExpression(Expression left, Expression right)
        {
            return Expression.Call(_objectEqualsMethod, left, right);
        }

        private static Expression MakeGetHashCodeExpression(FieldInfo field, UnaryExpression xParam)
        {
            return Expression.Call(Expression.Field(xParam, field), "GetHashCode", Type.EmptyTypes);
        }
    }
}