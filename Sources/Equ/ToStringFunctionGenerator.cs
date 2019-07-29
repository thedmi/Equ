namespace Equ
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Generator that produces ToString() functions for arbitrary objects. The functions are composed dynamically using reflection,
    /// but then compiled for high runtime performance.
    ///
    /// NOTE: This class is currently experimental.
    /// </summary>
    public class ToStringFunctionGenerator
    {                
        private static readonly MethodInfo _stringConcatMethod = typeof(string).GetTypeInfo().GetMethod("Concat", new[] { typeof(string[]) });
        
        private static readonly MethodInfo _objectToStringMethod = typeof(object).GetTypeInfo().GetMethod("ToString");

        private readonly Type _type;

        public ToStringFunctionGenerator(Type type)
        {
            _type = type;
        }

        /// <summary>
        /// Returns a compiled lambda that turns an object of type <see cref="_type"/> into its string representation.
        /// </summary>
        /// <returns></returns>
        public Func<object, string> MakeToStringMethod()
        {
            var fields = _type.GetTypeInfo().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            
            var toStringInputParam = Expression.Parameter(typeof(object));
            var concatExpr = AsObjectString(fields.Select(f => FieldStringRepresentation(f, toStringInputParam)).ToArray());

            // Wrap in null check for cases where the input param is already null
            var ifNotNullExpr = Expression.Condition(
                Expression.Equal(toStringInputParam, Expression.Constant(null)),
                Expression.Constant("∅"),
                concatExpr);
            
            return Expression.Lambda<Func<object, string>>(ifNotNullExpr, toStringInputParam).Compile();
        }
        
        /// <summary>
        /// Returns an expression of a string that contains the field name and value.
        /// </summary>
        private Expression FieldStringRepresentation(FieldInfo field, Expression instance)
        {
            var fieldName = Expression.Constant(GetDisplayName(field) + ": ");
            var fieldValue = FieldStringValue(field, instance);

            return Expression.Call(_stringConcatMethod, Expression.NewArrayInit(typeof(string), fieldName, fieldValue));
        }

        private Expression FieldStringValue(FieldInfo field, Expression instance)
        {
            var typedInstance = Expression.Convert(instance, _type);
            var fieldAccess = Expression.MakeMemberAccess(typedInstance, field);

            var fieldValue = FieldStringValue(fieldAccess, field.FieldType);
            
            // Wrap in null check expression
            return Expression.Condition(
                Expression.Equal(Expression.Convert(fieldAccess, typeof(object)), Expression.Constant(null)),
                Expression.Constant("∅"),
                fieldValue);
        }

        private static Expression FieldStringValue(Expression fieldAccess, Type fieldType)
        {
            if (ReflectionUtils.IsSequenceType(fieldType))
            {
                // The field is a sequence, so call ToString on each element and concatenate
                var enumerableToStringExpr = (Expression<Func<IEnumerable, string>>)(xs =>
                    "[ " + string.Join(", ", xs.Cast<object>().Select(x => x.ToString())) + " ]");

                return Expression.Invoke(enumerableToStringExpr, fieldAccess);
            }

            // The field is a scalar, so just call ToString on it
            return Expression.Call(Expression.Convert(fieldAccess, typeof(object)), _objectToStringMethod);
        }

        /// <summary>
        /// Concatenates the individual string expressions and encloses them in curly braces.
        /// </summary>
        private static Expression AsObjectString(params Expression[] stringExpressions)
        {
            var concatArgsExpr = Expression.NewArrayInit(typeof(string), stringExpressions);
            
            var joinAllAsObj = (Expression<Func<IEnumerable<string>, string>>)(ss => "{ " + string.Join(", ", ss) + " }");

            return Expression.Invoke(joinAllAsObj, concatArgsExpr);
        }
        
        /// <summary>
        /// Returns a human readable name for the field. If the field is a compiler-generated backing field for an auto property,
        /// the property name is used. Otherwise the field name is used.
        /// </summary>
        private static string GetDisplayName(FieldInfo field)
        {
            var autoPropertyInfo = ReflectionUtils.GetPropertyForBackingField(field);
            return autoPropertyInfo == null ? field.Name : autoPropertyInfo.Name;
        }
    }
}