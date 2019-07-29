namespace Equ.Test
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Xunit;

    public class ValueObjectWithDictionaryEqualityTest
    {
        [Fact]
        public void Equal_dicts_of_different_type_are_reported_equal()
        {
            var v1 = new DictValueObj(new Dictionary<int, string> { { 1, "a" }, { 2, "b" } });
            var v2 = new DictValueObj(new ReadOnlyDictionary<int, string>(new Dictionary<int, string> { { 1, "a" }, { 2, "b" } }));
            
            Assert.Equal(v1, v2);
            Assert.Equal(v1.GetHashCode(), v2.GetHashCode());
        }

        [Fact]
        public void Dictionary_order_is_disregarded_for_equality_comparison()
        {
            var v1 = new DictValueObj(new Dictionary<int, string> { { 1, "a" }, { 2, "b" } });
            var v2 = new DictValueObj(new Dictionary<int, string> { { 2, "b" }, { 1, "a" } });
            
            Assert.Equal(v1, v2);
            Assert.Equal(v1.GetHashCode(), v2.GetHashCode());
        }
        
        private class DictValueObj : MemberwiseEquatable<DictValueObj>
        {
            public DictValueObj(IReadOnlyDictionary<int, string> dict)
            {
                Dict = dict;
            }

            public IReadOnlyDictionary<int, string> Dict { get; }
        }
    }
}