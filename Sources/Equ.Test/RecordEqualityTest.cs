// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local
namespace Equ.Test
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    using Equ;

    using Xunit;

    public class RecordEqualityTest
    {
        [Fact]
        public void Self_equality_is_sane()
        {
            var x = new ValueType("asdf", 42, true, new CustomRefType("qwer"), new ValueType2("xyz"));

#pragma warning disable 1718
            // ReSharper disable EqualExpressionComparison
            Assert.True(x.Equals(x));
            Assert.True(x == x);
            Assert.False(x != x);
            Assert.False(x.Equals(null));
            Assert.False(Equals(null, x));

            // ReSharper disable once HeuristicUnreachableCode
            Assert.Equal(x.GetHashCode(), x.GetHashCode());
            // ReSharper restore EqualExpressionComparison
#pragma warning restore 1718
        }

        [Fact]
        public void Value_objects_with_equal_members_are_equal()
        {
            var customRefType = new CustomRefType("qwer");
            var x = new ValueType("asdf", 42, true, customRefType, new ValueType2("xyz"));
            var y = new ValueType("asdf", 42, false, customRefType, new ValueType2("xyz"));

            Assert.True(x.Equals(y));
            Assert.True(y.Equals(x));
            Assert.True(x == y);
            Assert.False(x != y);

            Assert.Equal(x.GetHashCode(), y.GetHashCode());
        }

        [Fact]
        public void Sequences_are_compared_element_by_element()
        {
            var x = new ValueTypeWithSequence(ImmutableList.Create(new ValueType2("a"), new ValueType2("b")));
            var y = new ValueTypeWithSequence(ImmutableList.Create(new ValueType2("a"), new ValueType2("b")));

            Assert.True(x.Equals(y));
            Assert.True(y.Equals(x));
            Assert.True(x == y);
            Assert.False(x != y);

            Assert.Equal(x.GetHashCode(), y.GetHashCode());
        }

        [Fact]
        public void Null_values_do_not_throw_exceptions_in_equals_and_getHashCode()
        {
            var x = new ValueType("asdf", 42, true, null, new ValueType2("xyz"));
            var y = new ValueType("asdf", 42, true, null, new ValueType2("xyz"));

            Assert.True(x.Equals(y));
            Assert.True(y.Equals(x));
            Assert.True(x == y);
            Assert.False(x != y);

            Assert.Equal(x.GetHashCode(), y.GetHashCode());
        }

        [Fact]
        public void Hash_codes_distribute()
        {
            var ints = Enumerable.Range(0, 40).ToList();
            var objs = ints.Select(i => new ValueType3(i)).ToList();
            
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

        [Fact]
        public void Inherited_members_are_compared()
        {
            var x1 = new SubValueType1(12, 34);
            var x2 = new SubValueType1(12, 34);

            Assert.Equal(x1, x2);

            // WARNING: Base class members are not considered in the equality comparison, so the following object is considered
            // equal to x1 and x2. To work around this issue, add all equality comparison relevant members to the concrete classes
            // as well, or use a custom equality comparer.
            var y = new SubValueType1(99, 34);
            Assert.Equal(x1, y);
        }

        [Fact]
        public void Empty_value_types_dont_throw()
        {
            var x1 = new EmptyValueType();
            var x2 = new EmptyValueType();

            Assert.Equal(x1, x2);
        }

        [Fact]
        public void Ignoring_auto_properties_excludes_them_from_equality_comparison()
        {
            var v1 = new ValueType4(15, 62);
            var v2 = new ValueType4(15, 13);

            Assert.Equal(v1, v2);
        }

        // ReSharper disable NotAccessedField.Local
        private record ValueType
        {
            private readonly string _value1;

            private readonly int _value2;

            [MemberwiseEqualityIgnore]
            private readonly bool _value3;

            private readonly CustomRefType _value4;

            private readonly ValueType2 _value5;

            public ValueType(string value1, int value2, bool value3, CustomRefType value4, ValueType2 value5)
            {
                _value1 = value1;
                _value2 = value2;
                _value3 = value3;
                _value4 = value4;
                _value5 = value5;
            }
            
            public virtual bool Equals(ValueType other) => EquCompare<ValueType>.Equals(this, other);
            public override int GetHashCode() => EquCompare<ValueType>.GetHashCode(this);
        }

        private record ValueType2(string S)
        {
            public virtual bool Equals(ValueType2 other) => EquCompare<ValueType2>.Equals(this, other);
            public override int GetHashCode() => EquCompare<ValueType2>.GetHashCode(this);
        }

        private record ValueType3(int Value)
        {
            public virtual bool Equals(ValueType3 other) => EquCompare<ValueType3>.Equals(this, other);
            public override int GetHashCode() => EquCompare<ValueType3>.GetHashCode(this);
        }

        private record ValueType4
        {
            public ValueType4(int value, int ignoredValue)
            {
                Value = value;
                IgnoredValue = ignoredValue;
            }

            public int Value { get; }

            [MemberwiseEqualityIgnore]
            public int IgnoredValue { get; }
            
            public virtual bool Equals(ValueType4 other) => EquCompare<ValueType4>.Equals(this, other);
            public override int GetHashCode() => EquCompare<ValueType4>.GetHashCode(this);
        }

        private record DictionaryType(IDictionary<string, string> Dict)
        {
            public virtual bool Equals(DictionaryType other) => EquCompare<DictionaryType>.Equals(this, other);
            public override int GetHashCode() => EquCompare<DictionaryType>.GetHashCode(this);
        }

        private record CustomRefType
        {
            private readonly string _s;

            public CustomRefType(string s)
            {
                _s = s;
            }
            
            public virtual bool Equals(CustomRefType other) => EquCompare<CustomRefType>.Equals(this, other);
            public override int GetHashCode() => EquCompare<CustomRefType>.GetHashCode(this);
        }

        private record ValueTypeWithSequence
        {
            private IReadOnlyList<ValueType2> _values;

            public ValueTypeWithSequence(IReadOnlyList<ValueType2> values)
            {
                _values = values;
            }
            
            public virtual bool Equals(ValueTypeWithSequence other) => EquCompare<ValueTypeWithSequence>.Equals(this, other);
            public override int GetHashCode() => EquCompare<ValueTypeWithSequence>.GetHashCode(this);
        }

        private abstract record AbstractValueType<TSelf>
        {
            private readonly int _val1;

            protected AbstractValueType(int val1)
            {
                _val1 = val1;
            }
            
            public virtual bool Equals(AbstractValueType<TSelf> other) => EquCompare<AbstractValueType<TSelf>>.Equals(this, other);
            public override int GetHashCode() => EquCompare<AbstractValueType<TSelf>>.GetHashCode(this);
        }

        private record EmptyValueType
        {
            public virtual bool Equals(EmptyValueType other) => EquCompare<EmptyValueType>.Equals(this, other);
            public override int GetHashCode() => EquCompare<EmptyValueType>.GetHashCode(this);
        }

        private record SubValueType1 : AbstractValueType<SubValueType1>
        {
            private readonly int _val2;

            public SubValueType1(int val1, int val2)
                : base(val1)
            {
                _val2 = val2;
            }
            
            public virtual bool Equals(SubValueType1 other) => EquCompare<SubValueType1>.Equals(this, other);
            public override int GetHashCode() => EquCompare<SubValueType1>.GetHashCode(this);
        }

        private record SubValueType2 : AbstractValueType<SubValueType2>
        {
            private readonly int _val2;

            public SubValueType2(int val1, int val2)
                : base(val1)
            {
                _val2 = val2;
            }
            
            public virtual bool Equals(SubValueType2 other) => EquCompare<SubValueType2>.Equals(this, other);
            public override int GetHashCode() => EquCompare<SubValueType2>.GetHashCode(this);
        }

        // ReSharper restore NotAccessedField.Local
    }
}
