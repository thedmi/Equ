namespace Equ
{
    using System;

    /// <summary>
    /// Tells the <see cref="MemberwiseEqualityComparer{T}" /> to ignore a specific field or property when performing
    /// the comparison. The marked member is also excluded from the hash code computation.
    /// Note that you'll have to filter members with this attribute yourself when you use a custom <see cref="EqualityFunctionGenerator"/>
    /// with <see cref="MemberwiseEqualityComparer{T}.Custom"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class MemberwiseEqualityIgnoreAttribute : Attribute
    {
    }
}