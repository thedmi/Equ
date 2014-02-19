// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFinalList.cs" company="Dani Michel">
//   Dani Michel 2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Belt.FinalList
{
    using System.Collections.Generic;
    using System.Collections.Immutable;

    /// <summary>
    /// A covariant, immutable list with eager-loading semantics.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFinalList<out T> : IReadOnlyList<T>
    {
        bool IsEmpty { get; }
    }
}
