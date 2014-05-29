// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="Dani Michel">
//   Dani Michel 2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Belt.Maybe
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Belt.FinalList;
    using Belt.Guard;

    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns the first item in the sequence wrapped in a <see cref="IMaybe{T}"/>, or an empty <see cref="IMaybe{T}"/> 
        /// if the sequence is empty.
        /// </summary>
        public static Maybe<T> FirstAsMaybe<T>(this IEnumerable<T> source)
        {
            try
            {
                return Maybe.Is(source.First());
            }
            catch (InvalidOperationException)
            {
                return Maybe.Empty<T>();
            }
        }

        /// <summary>
        /// Returns the first item in the sequence that matches the given predicate wrapped in a <see cref="IMaybe{T}"/>, 
        /// or an empty <see cref="IMaybe{T}"/>  if no item matches.
        /// </summary>
        public static Maybe<T> FirstAsMaybe<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            try
            {
                return Maybe.Is(source.First(predicate));
            }
            catch (InvalidOperationException)
            {
                return Maybe.Empty<T>();
            }
        }

        /// <summary>
        /// Returns the one and only item in the sequence wrapped in a <see cref="IMaybe{T}"/>, or an empty <see cref="IMaybe{T}"/> 
        /// if the sequence is empty. If the sequence contains more than one item, an <see cref="InvalidOperationException"/> is
        /// thrown.
        /// </summary>
        /// <exception cref="InvalidOperationException">There was more than one item in the sequence.</exception>
        public static Maybe<T> SingleAsMaybe<T>(this IEnumerable<T> source)
        {
            var firstTwo = source.Take(2).ToFinalList();
            return SingleAsMaybeCandidateToResult(firstTwo);
        }

        /// <summary>
        /// Returns the one and only item in the sequence that matches the given predicate wrapped in a <see cref="IMaybe{T}"/>, 
        /// or an empty <see cref="IMaybe{T}"/> if there is none. 
        /// If the sequence contains more than one item that matches, an <see cref="InvalidOperationException"/> is
        /// thrown.
        /// </summary>
        /// <exception cref="InvalidOperationException">There was more than one matching item in the sequence.</exception>
        public static Maybe<T> SingleAsMaybe<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            var firstTwoMatching = source.Where(predicate).Take(2).ToFinalList();
            return SingleAsMaybeCandidateToResult(firstTwoMatching);
        }

        private static Maybe<T> SingleAsMaybeCandidateToResult<T>(IFinalList<T> firstTwo)
        {
            Guard.Precondition(firstTwo.Count <= 2);

            switch (firstTwo.Count)
            {
                case 0:
                    return Maybe.Empty<T>();
                case 1:
                    return Maybe.Is(firstTwo[0]);
                default:
                    throw new InvalidOperationException("Source contains more than one element");
            }
        }

        /// <summary>
        /// Reduces an <c>IEnumerable</c> of <see cref="IMaybe{T}"/> to an <see cref="IEnumerable{T}"/>, where
        /// existing maybes from the original sequence are kept and empty ones are left out.
        /// </summary>
        public static IEnumerable<T> ExistingOnly<T>(this IEnumerable<IMaybe<T>> source)
        {
            return source.SelectMany(m => m.AsList());
        }

        /// <summary>
        /// Reduces an <c>IEnumerable</c> of <see cref="IMaybe{T}"/> to an <see cref="IEnumerable{T}"/>, where
        /// existing maybes from the original sequence are kept and empty ones are left out.
        /// </summary>
        public static IEnumerable<T> ExistingOnly<T>(this IEnumerable<Maybe<T>> source)
        {
            return ExistingOnly(source.Cast<IMaybe<T>>());
        }

        /// <summary>
        /// Looks up a key and returns the associated value wrapped in an <see cref="IMaybe{T}"/>,
        /// or an empty <see cref="IMaybe{T}"/> if the key doesn't exist.
        /// </summary>
        public static Maybe<TValue> Get<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            TValue value;
            var exists = source.TryGetValue(key, out value);

            return exists ? Maybe.Is(value) : Maybe.Empty<TValue>();
        } 
    }
}
