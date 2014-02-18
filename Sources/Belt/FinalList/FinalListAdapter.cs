// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FinalListAdapter.cs" company="Dani Michel">
//   Dani Michel 2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Belt.FinalList
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Immutable;

    internal sealed class FinalListAdapter<T> : IFinalList<T>
    {
        private static readonly FinalListAdapter<T> _empty = new FinalListAdapter<T>(ImmutableList<T>.Empty);

        private readonly ImmutableList<T> _list;

        public FinalListAdapter(params T[] values)
        {
            _list = System.Collections.Immutable.ImmutableList.Create(values);
        }

        public FinalListAdapter(ImmutableList<T> immutableList)
        {
            _list = immutableList;
        }

        public static FinalListAdapter<T> Empty { get { return _empty; } }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T this[int index] { get { return _list[index]; } }

        public int Count { get { return _list.Count; } }

        public bool IsEmpty { get { return _list.IsEmpty; } }

        public ImmutableList<T> ImmutableList { get { return _list; } }
    }
}