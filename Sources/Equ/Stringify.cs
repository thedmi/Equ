namespace Equ
{
    using System;

    public static class Stringify<T>
    {
        // This is a static field on purpose to make it initialize at static initialization time
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