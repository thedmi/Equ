namespace Equ.Test
{
    using System.Collections.Generic;

    using Equ;

    using Xunit;

    public class ElementwiseSequenceEqualityComparerTest
    {
        [Fact]
        public void Equal_sequences_are_reported_equal()
        {
            var e1 = new[] { "abc", "cde" };
            var e2 = new List<string> { "abc", "cde" };

            var comparer = new ElementwiseSequenceEqualityComparer<IEnumerable<string>>();

            Assert.True(comparer.Equals(e1, e1));
            Assert.True(comparer.Equals(e1, e2));
            Assert.True(comparer.Equals(e2, e1));
            Assert.True(comparer.Equals(e2, e2));

            Assert.Equal(comparer.GetHashCode(e1), comparer.GetHashCode(e2));
        }

        [Fact]
        public void Differing_lengths_are_reported_different()
        {
            var e1 = new[] { "abc", "cde" };
            var e2 = new[] { "abc", "cde", "" };

            var comparer = new ElementwiseSequenceEqualityComparer<IEnumerable<string>>();

            Assert.False(comparer.Equals(e1, e2));
            Assert.False(comparer.Equals(e2, e1));
        }

        [Fact]
        public void Null_values_are_reported_equal()
        {
            var e1 = new string[] { null };
            var e2 = new List<string> { null };

            var comparer = new ElementwiseSequenceEqualityComparer<IEnumerable<string>>();

            Assert.True(comparer.Equals(e1, e1));
            Assert.True(comparer.Equals(e1, e2));
            Assert.True(comparer.Equals(e2, e1));
            Assert.True(comparer.Equals(e2, e2));

            Assert.Equal(comparer.GetHashCode(e1), comparer.GetHashCode(e2));
        }

        [Fact]
        public void Null_and_non_null_values_are_reported_different()
        {
            var e1 = new[] { "abc" };
            var e2 = new List<string> { null };

            var comparer = new ElementwiseSequenceEqualityComparer<IEnumerable<string>>();

            Assert.False(comparer.Equals(e1, e2));
            Assert.False(comparer.Equals(e2, e1));
        }
    }
}
