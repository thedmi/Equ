// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILazy.cs" company="Dani Michel">
//   Dani Michel 2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Belt.Lazy
{
    public interface ILazy<out T> : ILazy
    {
        T Value { get; }
    }

    public interface ILazy { }
}
