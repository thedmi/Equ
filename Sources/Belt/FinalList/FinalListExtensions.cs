// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FinalListExtensions.cs" company="Dani Michel">
//   Dani Michel 2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Belt.FinalList
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    public static class FinalListExtensions
    {
        public static IFinalList<T> ToFinalList<T>(this IEnumerable<T> source)
        {
            var finalList = source as IFinalList<T>;
            if (finalList != null)
            {
                return finalList;
            }

            var immutableList = source as ImmutableList<T>;
            if (immutableList != null)
            {
                return new FinalListAdapter<T>(immutableList);
            }

            return new FinalListAdapter<T>(ImmutableList.CreateRange(source));
        }

        public static int Count<T>(this IFinalList<T> source)
        {
            return source.TryTakeShortcut(e => e.Count());
        }

        public static T ElementAt<T>(this IFinalList<T> source, int index)
        {
            return source.TryTakeShortcut(e => e.ElementAt(index));
        }

        public static T Single<T>(this IFinalList<T> source)
        {
            return source.TryTakeShortcut(e => e.Single());
        }

        public static T Single<T>(this IFinalList<T> source, Func<T, bool> predicate)
        {
            return source.TryTakeShortcut(e => e.Single(predicate));
        }

        public static T SingleOrDefault<T>(this IFinalList<T> source)
        {
            return source.TryTakeShortcut(e => e.SingleOrDefault());
        }

        public static T First<T>(this IFinalList<T> source)
        {
            return source.TryTakeShortcut(e => e.First());
        }

        public static T First<T>(this IFinalList<T> source, Func<T, bool> predicate)
        {
            return source.TryTakeShortcut(e => e.First(predicate));
        }

        public static T FirstOrDefault<T>(this IFinalList<T> source)
        {
            return source.TryTakeShortcut(e => e.FirstOrDefault());
        }

        public static T Last<T>(this IFinalList<T> source)
        {
            return source.TryTakeShortcut(e => e.Last());
        }

        public static T LastOrDefault<T>(this IFinalList<T> source)
        {
            return source.TryTakeShortcut(e => e.LastOrDefault());
        }

        public static IEnumerable<T> Where<T>(this IFinalList<T> source, Func<T, bool> predicate)
        {
            return source.TryTakeShortcut(e => e.Where(predicate));
        }

        public static IEnumerable<TResult> Select<T, TResult>(this IFinalList<T> source, Func<T, TResult> selector)
        {
            return source.TryTakeShortcut(e => e.Select(selector));
        }

        private static TResult TryTakeShortcut<T, TResult>(this IFinalList<T> source, Func<IEnumerable<T>, TResult> function)
        {
            var adapter = source as FinalListAdapter<T>;
            if (adapter != null)
            {
                return function(adapter.ImmutableList);
            }
            return function(source);
        }
    }
}
