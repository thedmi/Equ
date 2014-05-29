// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MaybeExtensions.cs" company="Dani Michel">
//   Dani Michel 2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Belt.Maybe
{
    using System;

    public static class MaybeExtensions
    {
        public static T? AsNullable<T>(this IMaybe<T> maybe) where T : struct
        {
            return maybe.Exists ? maybe.It : (T?)null;
        }

        /// <summary>
        /// Maps the maybe to a new maybe of another type using the mapping function <paramref name="selector"/> while
        /// preserving the existing/empty state.
        /// </summary>
        public static Maybe<TResult> Select<T, TResult>(this IMaybe<T> maybe, Func<T, TResult> selector)
        {
            return maybe.IsEmpty ? Maybe.Empty<TResult>() : Maybe.Is(selector(maybe.It));
        }

        /// <summary>
        /// Calls the <paramref name="selector"/> with the value of this maybe if it exists, or returns an 
        /// empty maybe otherwise.
        /// </summary>
        public static Maybe<TResult> SelectMany<T, TResult>(this IMaybe<T> maybe, Func<T, Maybe<TResult>> selector)
        {
            return maybe.IsEmpty ? Maybe.Empty<TResult>() : selector(maybe.It);
        }

        public static Maybe<TResult> Cast<TResult>(this IMaybe maybe)
        {
            var baseMaybe = (IMaybe<object>)maybe;
            return maybe.IsEmpty ? Maybe.Empty<TResult>() : Maybe.Is((TResult)baseMaybe.It);
        }
    }
}
