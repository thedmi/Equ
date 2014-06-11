namespace BeltTest.Belt
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;

    using global::Belt.Equatable;
    using global::Belt.FinalList;
    using global::Belt.Maybe;

    using Xunit;

    public class MemberwiseEquatableTest
    {

        [Fact]
        public void Self_equality_is_sane()
        {
            var x = new ValueType("asdf", 42, true, new CustomRefType("qwer"), new ValueType2("xyz"), Maybe.Is(new ValueType3(4)));

            // ReSharper disable EqualExpressionComparison
            Assert.True(x.Equals(x));
            Assert.True(x == x);
            Assert.False(x != x);
            Assert.False(x.Equals(null));
            Assert.False(Equals(null, x));

            Assert.Equal(x.GetHashCode(), x.GetHashCode());
            // ReSharper restore EqualExpressionComparison
        }

        [Fact]
        public void Value_objects_with_equal_members_are_equal()
        {
            var customRefType = new CustomRefType("qwer");
            var x = new ValueType("asdf", 42, true, customRefType, new ValueType2("xyz"), Maybe.Is(new ValueType3(4)));
            var y = new ValueType("asdf", 42, false, customRefType, new ValueType2("xyz"), Maybe.Is(new ValueType3(4)));

            Assert.True(x.Equals(y));
            Assert.True(y.Equals(x));
            Assert.True(x == y);
            Assert.False(x != y);

            Assert.Equal(x.GetHashCode(), y.GetHashCode());
        }

        [Fact]
        public void Contained_value_types_are_compared_by_reference()
        { 
            // The second CustomRefType is different from the first
            var x = new ValueType("asdf", 42, true, new CustomRefType("qwer"), new ValueType2("xyz"), Maybe.Is(new ValueType3(4)));
            var y = new ValueType("asdf", 42, true, new CustomRefType("qwer"), new ValueType2("xyz"), Maybe.Is(new ValueType3(4)));

            Assert.False(x.Equals(y));
            Assert.False(y.Equals(x));
            Assert.False(x == y);
            Assert.True(x != y);

            Assert.NotEqual(x.GetHashCode(), y.GetHashCode());
        }

        [Fact]
        public void Sequences_are_compared_element_by_element()
        {
            var x = new ValueTypeWithSequence(FinalList.Create(new ValueType2("a"), new ValueType2("b")));
            var y = new ValueTypeWithSequence(FinalList.Create(new ValueType2("a"), new ValueType2("b")));

            Assert.True(x.Equals(y));
            Assert.True(y.Equals(x));
            Assert.True(x == y);
            Assert.False(x != y);

            Assert.Equal(x.GetHashCode(), y.GetHashCode());
        }

        [Fact]
        public void Null_values_do_not_throw_exceptions_in_equals_and_getHashCode()
        {
            var x = new ValueType("asdf", 42, true, null, new ValueType2("xyz"), Maybe.Is(new ValueType3(4)));
            var y = new ValueType("asdf", 42, true, null, new ValueType2("xyz"), Maybe.Is(new ValueType3(4)));

            Assert.True(x.Equals(y));
            Assert.True(y.Equals(x));
            Assert.True(x == y);
            Assert.False(x != y);

            Assert.Equal(x.GetHashCode(), y.GetHashCode());
        }

        [Fact]
        public void Hash_codes_distribute()
        {
            var ints = Enumerable.Range(0, 40).ToFinalList();
            var objs = ints.Select(i => new ValueType3(i)).ToFinalList();
            
            Assert.Equal(ints.Count, new HashSet<ValueType3>(objs).Count);
        }

        [Fact]
        public void Dictionary_members_are_used_correctly_for_equality_comparison()
        {
            var x = new DictionaryType(new Dictionary<string, string> { { "a", "The A" }, { "b", "The B" } });
            var y = new DictionaryType(new Dictionary<string, string> { { "a", "The A" }, { "b", "The B" } });

            Assert.True(x.Equals(y));
            Assert.True(y.Equals(x));
            Assert.True(x == y);
            Assert.False(x != y);

            Assert.Equal(x.GetHashCode(), y.GetHashCode());
        }

        // TODO Add sequence equality tests

        // ReSharper disable NotAccessedField.Local
        private class ValueType : MemberwiseEquatable<ValueType>
        {
            private readonly string _value1;

            private readonly int _value2;

            [MemberwiseEqualityIgnore]
            private readonly bool _value3;

            private readonly CustomRefType _value4;

            private readonly ValueType2 _value5;

            private readonly IMaybe<ValueType3> _value6;

            public ValueType(string value1, int value2, bool value3, CustomRefType value4, ValueType2 value5, IMaybe<ValueType3> value6)
            {
                _value1 = value1;
                _value2 = value2;
                _value3 = value3;
                _value4 = value4;
                _value5 = value5;
                _value6 = value6;
            }
        }

        private class ValueType2 : MemberwiseEquatable<ValueType2>
        {
            private readonly string _s;

            public ValueType2(string s)
            {
                _s = s;
            }
        }

        private class ValueType3 : ValueBasedEquatable<ValueType3, string>
        {
            private readonly int _value;

            public ValueType3(int value)
            {
                _value = value;
            }

            public int Value { get { return _value; } }

            protected override string EquatableValue { get { return Convert.ToString(_value); } }
        }

        private class DictionaryType : MemberwiseEquatable<DictionaryType>
        {
            private readonly IDictionary<string, string> _dict = new Dictionary<string, string>();

            public DictionaryType(IDictionary<string, string> dict)
            {
                _dict = dict;
            }
        }

        private class CustomRefType
        {
            private readonly string _s;

            public CustomRefType(string s)
            {
                _s = s;
            }
        }

        private class ValueTypeWithSequence : MemberwiseEquatable<ValueTypeWithSequence>
        {
            private IFinalList<ValueType2> _values;

            public ValueTypeWithSequence(IFinalList<ValueType2> values)
            {
                _values = values;
            }
        }
        // ReSharper restore NotAccessedField.Local
    }
}
