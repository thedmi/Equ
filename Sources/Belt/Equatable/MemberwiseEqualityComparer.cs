/*
 * Copied and adapted from https://github.com/niik/MemberwiseEqualityComparer/blob/master/MemberwiseEqualityComparer.cs
 */

/*
 * Copyright (c) 2008-2009 Markus Olsson
 * var mail = string.Join(".", new string[] {"j", "markus", "olsson"}) + string.Concat('@', "gmail.com");
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this 
 * software and associated documentation files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use, copy, modify, merge, publish, 
 * distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING 
 * BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

namespace Belt.Equatable
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    /// <summary>
    ///  Provides an implementation of EqualityComparer that performs memberwise
    ///  equality comparison of objects.
    /// </summary>
    public class MemberwiseEqualityComparer<T> : EqualityComparer<T>
    {
        private static readonly Func<T, T, bool> _equalityFunc;
        private static readonly Func<T, int> _hashCodeFunc;

        // ReSharper disable once UnusedMember.Global
        public static new MemberwiseEqualityComparer<T> Default
        {
            get { return new MemberwiseEqualityComparer<T>(); }
        }

        static MemberwiseEqualityComparer()
        {
            var targetType = typeof(T);

            var targetFieldMembers = targetType
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(fi => fi.GetCustomAttributes(typeof(MemberwiseEqualityIgnoreAttribute), true).Length == 0)
                .ToArray();

            _equalityFunc = BuildDynamicEqualityMethod(targetType, targetFieldMembers);
            _hashCodeFunc = BuildDynamicHashCodeMethod(targetType, targetFieldMembers);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberwiseEqualityComparer&lt;T&gt;"/> class.
        /// </summary>
        public MemberwiseEqualityComparer()
        {
        }

        /// <summary>
        /// When overridden in a derived class, determines whether two objects of type <paramtyperef name="T"/> are equal.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public override bool Equals(T x, T y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }
            if (ReferenceEquals(x, null))
            {
                return false;
            }
            if (ReferenceEquals(y, null))
            {
                return false;
            }
            return _equalityFunc(x, y);
        }

        /// <summary>
        /// Serves as a hash function for the specified object for hashing algorithms and data structures, such as a hash table.
        /// </summary>
        /// <param name="obj">The object to calculate a hash code for.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.
        /// </exception>
        public override int GetHashCode(T obj)
        {
            if (ReferenceEquals(obj, null))
                return 0;

            return _hashCodeFunc(obj);
        }

        /// <summary>
        /// Builds the dynamic hash code method.
        /// </summary>
        private static Func<T, int> BuildDynamicHashCodeMethod(Type targetType, FieldInfo[] targetFieldMembers)
        {
            // If there's no members available for us to calculate hash code with we default to 
            // zero. Note that we cannot simply call the GetHashCode method of the T instance here
            // since it very will lead to a never ending recursing if the T GetHashCode method calls
            // MemberwiseEqualityComparer<T>.Default.GetHashCode(this).
            if (targetFieldMembers.Length == 0)
                return x => 0;

            var dynamicHashCodeMethod = new DynamicMethod("DynamicGetHashCode", typeof(int), new[] { targetType }, typeof(MemberwiseEqualityComparer<T>), true);

            var ilGenerator = dynamicHashCodeMethod.GetILGenerator();

            // Load a prime number as starting point.
            ilGenerator.Emit(OpCodes.Ldc_I4_7);

            var typeHistory = new HashSet<Type>();

            var equalityComparerGetters = new Dictionary<Type, MethodInfo>();
            var equalityComparerHashCodeMethods = new Dictionary<Type, MethodInfo>();

            foreach (var fi in targetFieldMembers)
            {
                Type memberType = fi.FieldType;

                MethodInfo defaultEqualityGetter;
                MethodInfo equalityComparerHashCodeMethod;

                if (!typeHistory.Contains(memberType))
                {
                    typeHistory.Add(memberType);

                    var genericEqualityComparer = GetEqualityComparer(memberType);
                    PropertyInfo defaultComparerProperty = genericEqualityComparer.GetProperty("Default", genericEqualityComparer);

                    defaultEqualityGetter = defaultComparerProperty.GetGetMethod();
                    equalityComparerHashCodeMethod = genericEqualityComparer.GetMethod("GetHashCode", new[] { memberType });

                    equalityComparerGetters.Add(memberType, defaultEqualityGetter);
                    equalityComparerHashCodeMethods.Add(memberType, equalityComparerHashCodeMethod);
                }
                else
                {
                    defaultEqualityGetter = equalityComparerGetters[memberType];
                    equalityComparerHashCodeMethod = equalityComparerHashCodeMethods[memberType];
                }

                ilGenerator.EmitCall(OpCodes.Call, defaultEqualityGetter, null);

                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldfld, fi);

                ilGenerator.EmitCall(OpCodes.Callvirt, equalityComparerHashCodeMethod, null);
                ilGenerator.Emit(OpCodes.Xor);
            }

            ilGenerator.Emit(OpCodes.Ret);

            return (Func<T, int>)dynamicHashCodeMethod.CreateDelegate(typeof(Func<T, int>));
        }

        /// <summary>
        /// Builds the dynamic equality method.
        /// </summary>
        private static Func<T, T, bool> BuildDynamicEqualityMethod(Type targetType, FieldInfo[] targetFieldMembers)
        {
            // In case there are no fields we consider the objects to be equal
            if (targetFieldMembers.Length == 0)
                return (x, y) => true;

            var equalityMethod = new DynamicMethod("DynamicEquals", typeof(bool), new[] { targetType, targetType }, typeof(MemberwiseEqualityComparer<T>), true);

            var ilGenerator = equalityMethod.GetILGenerator();
            var notEqualLabel = ilGenerator.DefineLabel();

            var typeHistory = new HashSet<Type>();

            var equalityComparerGetters = new Dictionary<Type, MethodInfo>();
            var concreteEqualsMethods = new Dictionary<Type, MethodInfo>();

            var referenceEquals = typeof(object).GetMethod("ReferenceEquals", new[] { typeof(object), typeof(object) });

            foreach (var fi in targetFieldMembers)
            {
                Type memberType = fi.FieldType;

                MethodInfo propertyGetMethod;
                MethodInfo concreteEqualsMethod;

                if (!typeHistory.Contains(memberType))
                {
                    typeHistory.Add(memberType);

                    var genericEqualityComparer = GetEqualityComparer(memberType);
                    var defaultComparerProperty = genericEqualityComparer.GetProperty("Default", genericEqualityComparer);

                    propertyGetMethod = defaultComparerProperty.GetGetMethod();
                    concreteEqualsMethod = genericEqualityComparer.GetMethod("Equals", new[] { memberType, memberType });

                    equalityComparerGetters.Add(memberType, propertyGetMethod);
                    concreteEqualsMethods.Add(memberType, concreteEqualsMethod);
                }
                else
                {
                    propertyGetMethod = equalityComparerGetters[memberType];
                    concreteEqualsMethod = concreteEqualsMethods[memberType];
                }

                var skip = ilGenerator.DefineLabel();

                // Performance trick: Skip the real Equals() call on the EqualityProvider if where're dealing
                // with the same object. Has the added benefit of making null comparisons really fast.
                if (!memberType.IsValueType)
                {
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldfld, fi);

                    ilGenerator.Emit(OpCodes.Ldarg_1);
                    ilGenerator.Emit(OpCodes.Ldfld, fi);

                    ilGenerator.EmitCall(OpCodes.Call, referenceEquals, null);
                    ilGenerator.Emit(OpCodes.Brtrue, skip);
                }

                ilGenerator.EmitCall(OpCodes.Call, propertyGetMethod, null);

                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldfld, fi);

                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.Emit(OpCodes.Ldfld, fi);

                ilGenerator.EmitCall(OpCodes.Callvirt, concreteEqualsMethod, null);
                ilGenerator.Emit(OpCodes.Brfalse, notEqualLabel);

                ilGenerator.MarkLabel(skip);
            }

            ilGenerator.Emit(OpCodes.Ldc_I4_1);
            ilGenerator.Emit(OpCodes.Ret);

            ilGenerator.MarkLabel(notEqualLabel);
            ilGenerator.Emit(OpCodes.Ldc_I4_0);
            ilGenerator.Emit(OpCodes.Ret);

            return (Func<T, T, bool>)equalityMethod.CreateDelegate(typeof(Func<T, T, bool>));
        }

        private static Type GetEqualityComparer(Type memberType)
        {
            var equalityComparerType = typeof(IEnumerable).IsAssignableFrom(memberType) && memberType != typeof(string)
                ? typeof(ElementwiseSequenceEqualityComparer<>)
                : typeof(EqualityComparer<>);

            return equalityComparerType.MakeGenericType(new[] { memberType });
        }
    }
}
