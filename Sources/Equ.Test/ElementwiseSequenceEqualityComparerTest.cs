namespace Equ.Test
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;

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
        
        [Fact]
        public void Equal_dicts_of_different_type_are_reported_equal()
        {
            var dict1 = new Dictionary<int, string> { { 1, "a" }, { 2, "b" } };
            var dict2 = new ReadOnlyDictionary<int, string>(new Dictionary<int, string> { { 1, "a" }, { 2, "b" } });
            
            var comparer = new ElementwiseSequenceEqualityComparer<IReadOnlyDictionary<int, string>>();
            
            Assert.True(comparer.Equals(dict1, dict2));
            Assert.Equal(comparer.GetHashCode(dict1), comparer.GetHashCode(dict2));
        }
        
        [Fact]
        public void ReadOnlyDictionary_order_is_disregarded_for_equality_comparison()
        {
            var dict1 = new Dictionary<int, string> { { 1, "a" }, { 2, "b" } };
            var dict2 = new Dictionary<int, string> { { 2, "b" }, { 1, "a" } };
            
            var comparer = new ElementwiseSequenceEqualityComparer<IReadOnlyDictionary<int, string>>();
            
            Assert.True(comparer.Equals(dict1, dict2));
            Assert.Equal(comparer.GetHashCode(dict1), comparer.GetHashCode(dict2));
        }
        
        [Fact]
        public void Dictionary_order_is_disregarded_for_equality_comparison()
        {
            var dict1 = new Dictionary<int, string> { { 1, "a" }, { 2, "b" } };
            var dict2 = new Dictionary<int, string> { { 2, "b" }, { 1, "a" } };
            
            var comparer = new ElementwiseSequenceEqualityComparer<IDictionary>();
            
            Assert.True(comparer.Equals(dict1, dict2));
            Assert.Equal(comparer.GetHashCode(dict1), comparer.GetHashCode(dict2));
        }
        
        [Fact]
        public void Different_dictionaries_by_value_are_reported_unequal()
        {
            var dict1 = new Dictionary<int, string> { { 1, "a" }, { 2, "b" } };
            var dict2 = new Dictionary<int, string> { { 1, "a" }, { 2, "c" } };
            
            var comparer = new ElementwiseSequenceEqualityComparer<IDictionary>();
            
            Assert.False(comparer.Equals(dict1, dict2));
            Assert.NotEqual(comparer.GetHashCode(dict1), comparer.GetHashCode(dict2));
        }
        
        [Fact]
        public void Different_dictionaries_by_key_are_reported_unequal()
        {
            var dict1 = new Dictionary<int, string> { { 1, "a" }, { 2, "b" } };
            var dict2 = new Dictionary<int, string> { { 1, "a" }, { 3, "b" } };
            
            var comparer = new ElementwiseSequenceEqualityComparer<IDictionary>();
            
            Assert.False(comparer.Equals(dict1, dict2));
            Assert.NotEqual(comparer.GetHashCode(dict1), comparer.GetHashCode(dict2));
        }
        
        [Fact]
        public void Set_order_is_disregarded_for_equality_comparison()
        {
            var set1 = new HashSet<int> { 1, 2 };
            var set2 = new HashSet<int> { 2, 1 };
            var comparer = new ElementwiseSequenceEqualityComparer<ISet<int>>();
            
            Assert.True(comparer.Equals(set1, set2));
            Assert.Equal(comparer.GetHashCode(set1), comparer.GetHashCode(set2));
        }
        
        [Fact]
        public void Different_sets_are_reported_unequal()
        {
            var set1 = new HashSet<int> { 1, 2 };
            var set2 = new HashSet<int> { 1, 3 };
            var comparer = new ElementwiseSequenceEqualityComparer<ISet<int>>();
            
            Assert.False(comparer.Equals(set1, set2));
            Assert.NotEqual(comparer.GetHashCode(set1), comparer.GetHashCode(set2));
        }
    }
}
