// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMaybe.cs" company="Dani Michel">
//   Dani Michel 2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Belt.Maybe
{
    using System;

    using Belt.FinalList;

    /// <summary>
    /// Represents values of type <typeparamref name="T"/> that may or may not exist.
    /// </summary>
    public interface IMaybe<out T>
    {
        /// <summary>
        /// Returns the actual value of the Maybe and throws if it is empty.
        /// </summary>
        T It { get; }

        /// <summary>
        /// Returns the value of the Maybe if it exists, null otherwise. This getter
        /// is primarily designed for use with the null coalescing operator.
        /// </summary>
        T ItOrDefault { get; }

        /// <summary>
        /// Returns <see cref="It"/> or throws <paramref name="exception"/> if it doesn't exist.
        /// </summary>
        T ItOrThrow(Exception exception);

        /// <summary>
        /// Returns true if there is no value in this maybe.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Returns true if a value exists and accessing <see cref="It"/> is safe.
        /// </summary>
        bool Exists { get; }

        /// <summary>
        /// Returns the maybe as immutable list of T. This list has the length of one if the maybe
        /// exists, or zero if the maybe is empty.
        /// </summary>
        /// <returns>An immutable list of length one or zero.</returns>
        IFinalList<T> AsList();

        /// <summary>
        /// Maps the maybe to a new maybe of another type using the mapping function <paramref name="selector"/> while
        /// preserving the existing/empty state.
        /// </summary>
        IMaybe<TResult> Select<TResult>(Func<T, TResult> selector);

        /// <summary>
        /// Calls the <paramref name="selector"/> with the value of this maybe if it exists, or returns an 
        /// empty maybe otherwise.
        /// </summary>
        IMaybe<TResult> SelectMany<TResult>(Func<T, IMaybe<TResult>> selector);
    }
}