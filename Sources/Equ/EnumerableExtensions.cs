namespace Equ
{
    using System;
    using System.Collections.Generic;

    // TODO Remove before stable version 
    [Obsolete]
    public static class EnumerableExtensions
    {
        [Obsolete]
        public static EquatableSequenceWrapper<T> AsEquatableSequence<T>(this IEnumerable<T> source)
        {
            return new EquatableSequenceWrapper<T>(source);
        }
    }
}