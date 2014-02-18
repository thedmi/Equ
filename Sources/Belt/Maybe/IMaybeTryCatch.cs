// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMaybeTryCatch.cs" company="Dani Michel">
//   Dani Michel 2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Belt.Maybe
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public interface IMaybeTryCatch<out T> 
    {
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Catch",
            Justification = "The similarity to the reserved keyword 'catch' is intended.")]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Providing the excplicit exception type is the whole point of this method.")]
        IMaybe<T> Catch<TException>() where TException : Exception;
    }
}