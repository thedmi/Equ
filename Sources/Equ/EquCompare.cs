namespace Equ
{
    /// <summary>
    /// A convenience class that enables simple Equ usage without inheritance. This is also the way to go with C#9 records.
    /// </summary>
    /// <example>
    /// record Some(string Value)
    /// {
    ///     public virtual bool Equals(Some other) => EquCompare&lt;Some&gt;.Equals(this, other);
    ///     public override int GetHashCode() => EquCompare&lt;Some&gt;.GetHashCode(this);
    /// }
    /// </example>
    public static class EquCompare<T>
    {
        private static readonly MemberwiseEqualityComparer<T> _comparer = MemberwiseEqualityComparer<T>.ByFields;

        public static bool Equals(T self, T other) => _comparer.Equals(self, other);

        public static int GetHashCode(T self) => _comparer.GetHashCode(self);
    }
}