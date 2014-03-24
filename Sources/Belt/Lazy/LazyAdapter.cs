// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LazyAdapter.cs" company="Dani Michel">
//   Dani Michel 2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Belt.Lazy
{
    using System;

    internal class LazyAdapter<T> : ILazy<T>
    {
        private readonly Lazy<T> _lazy;

        public LazyAdapter(Func<T> valueProducer)
        {
            _lazy = new Lazy<T>(valueProducer);
        }

        public T Value { get { return _lazy.Value; } }
    }
}