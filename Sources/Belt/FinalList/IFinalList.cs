// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFinalList.cs" company="Dani Michel">
//   Dani Michel 2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Belt.FinalList
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// A covariant, immutable list with eager-loading semantics.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFinalList<out T> : IReadOnlyList<T>, IFinalList
    {
    }

    /// <summary>
    /// A non-generic immutable list. Use <see cref="IFinalList{T}"/> where possible.
    /// </summary>
    public interface IFinalList : IEnumerable
    {
        bool IsEmpty { get; }   
    }
}
