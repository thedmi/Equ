namespace Equ
{
    using System;

    /// <summary>
    /// Tells the <see cref="MemberwiseEqualityComparer{T}" /> to ignore a specific field when performing
    /// the comparison. The field is also excluded from the hashcode computation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class MemberwiseEqualityIgnoreAttribute : Attribute
    {
    }
}