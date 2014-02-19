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
        public static IMaybe<T> FirstAsMaybe<T>(this IEnumerable<T> source)
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

        public static IMaybe<T> FirstAsMaybe<T>(this IEnumerable<T> source, Func<T, bool> predicate)
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

        public static IMaybe<T> SingleAsMaybe<T>(this IEnumerable<T> source)
        {
            var firstTwo = source.Take(2).ToFinalList();
            return SingleAsMaybeCandidateToResult(firstTwo);
        }

        public static IMaybe<T> SingleAsMaybe<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            var firstTwoMatching = source.Where(predicate).Take(2).ToFinalList();
            return SingleAsMaybeCandidateToResult(firstTwoMatching);
        }

        private static IMaybe<T> SingleAsMaybeCandidateToResult<T>(IFinalList<T> firstTwo)
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
    }
}
