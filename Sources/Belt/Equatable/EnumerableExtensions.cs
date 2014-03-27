namespace Belt.Equatable
{
    using System.Collections.Generic;

    public static class EnumerableExtensions
    {
        public static EquatableSequenceWrapper<T> AsEquatableSequence<T>(this IEnumerable<T> source)
        {
            return new EquatableSequenceWrapper<T>(source);
        }
    }
}