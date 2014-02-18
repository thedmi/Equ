// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MaybeExtensions.cs" company="Dani Michel">
//   Dani Michel 2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Belt.Maybe
{
    public static class MaybeExtensions
    {
        public static T? AsNullable<T>(this IMaybe<T> maybe) where T : struct
        {
            return maybe.Exists ? maybe.It : (T?)null;
        }
    }
}
