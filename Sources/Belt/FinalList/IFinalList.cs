// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFinalList.cs" company="Dani Michel">
//   Dani Michel 2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Belt.FinalList
{
    using System.Collections.Generic;

    public interface IFinalList<out T> : IReadOnlyList<T>
    {
        bool IsEmpty { get; }
    }
}
