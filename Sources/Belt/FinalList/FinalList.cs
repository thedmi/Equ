// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FinalList.cs" company="Dani Michel">
//   Dani Michel 2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Belt.FinalList
{
    public static class FinalList
    {
        public static IFinalList<T> Create<T>(params T[] values)
        {
            return new FinalListAdapter<T>(values);
        }

        public static IFinalList<T> Empty<T>()
        {
            return FinalListAdapter<T>.Empty;
        }
    }
}