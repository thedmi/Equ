namespace Equ
{
    using System;

    /// <summary>
    /// Convenience wrapper around <see cref="ToStringFunctionGenerator"/>. *This is currently experimental.*
    /// </summary>
    public static class Stringify<T>
    {
        // This is deliberately a static field to make it initialize at static initialization time
        private static readonly Func<object, string> _toStringMethod = new ToStringFunctionGenerator(typeof(T)).MakeToStringMethod();

        /// <summary>
        /// Returns a string representation of <paramref name="obj"/> using <see cref="ToStringFunctionGenerator"/>.
        /// </summary>
        public static string With(T obj)
        {
            return _toStringMethod(obj);
        }
    }
}