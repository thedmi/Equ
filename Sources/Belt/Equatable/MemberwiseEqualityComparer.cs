namespace Belt.Equatable
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    ///  Provides an implementation of EqualityComparer that performs memberwise
    ///  equality comparison of objects.
    /// </summary>
    public class MemberwiseEqualityComparer<T> : IEqualityComparer<T>
    {
        private static readonly Func<object, object, bool> _equalsFunc = MakeEqualsMethod(typeof(T));

        private static readonly Func<object, int> _getHashCodeFunc = MakeGetHashCodeMethod(typeof(T)); 
        
        public bool Equals(T x, T y)
        {
            return _equalsFunc(x, y);
        }

        public int GetHashCode(T obj)
        {
            return _getHashCodeFunc(obj);
        }

        private static IEnumerable<FieldInfo> GetIncludedMembers(Type type)
        {
            return
                type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(fi => fi.GetCustomAttributes(typeof(MemberwiseEqualityIgnoreAttribute), true).Length == 0);
        } 

        private static Func<object, int> MakeGetHashCodeMethod(Type type)
        {
            var objRaw = Expression.Parameter(typeof(object), "obj");

            // cast to the subclass type
            var objParam = Expression.Convert(objRaw, type);

            // compound XOr expression
            var getHashCodeExprs = GetIncludedMembers(type).Select(p => MakeGetHashCodeExpression(p, objParam));
            var xorChainExpr = getHashCodeExprs.Aggregate((Expression)Expression.Constant(0), Expression.ExclusiveOr);
            
            return Expression.Lambda<Func<object, int>>(xorChainExpr, objRaw).Compile();
        }

        private static Func<object, object, bool> MakeEqualsMethod(Type type)
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

        private static Expression MakeEqualsExpression(FieldInfo field, UnaryExpression xParam, UnaryExpression yParam)
        {
            var equalsMethod = typeof(object).GetMethod("Equals", BindingFlags.Static | BindingFlags.Public);

            var xField = Expression.Field(xParam, field);
            var yField = Expression.Field(yParam, field);

            return field.FieldType.IsValueType
                ? (Expression)Expression.Equal(xField, yField)
                : Expression.Call(equalsMethod, xField, yField);
        }

        private static Expression MakeGetHashCodeExpression(FieldInfo field, UnaryExpression xParam)
        {
            return Expression.Call(Expression.Field(xParam, field), "GetHashCode", Type.EmptyTypes);
        }
    }
}
