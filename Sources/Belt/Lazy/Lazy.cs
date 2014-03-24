// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Lazy.cs" company="Dani Michel">
//   Dani Michel 2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Belt.Lazy
{
    using System;

    public static class Lazy
    {
        public static ILazy<T> Create<T>(Func<T> valueProducer)
        {
            return new LazyAdapter<T>(valueProducer);
        }

        public static ILazy<T> CreateEager<T>(T eagerlyInitializedValue)
        {
            return new LazyAdapter<T>(() => eagerlyInitializedValue);
        }
    }
}
