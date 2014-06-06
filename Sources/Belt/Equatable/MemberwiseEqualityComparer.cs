namespace Belt.Equatable
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///  Provides an implementation of EqualityComparer that performs memberwise
    ///  equality comparison of objects.
    /// </summary>
    public class MemberwiseEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<object, object, bool> _equalsFunc = EqualityFunctionGenerator.MakeEqualsMethod(typeof(T));

        private readonly Func<object, int> _getHashCodeFunc = EqualityFunctionGenerator.MakeGetHashCodeMethod(typeof(T));
        
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
