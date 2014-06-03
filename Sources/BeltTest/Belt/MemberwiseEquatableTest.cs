namespace BeltTest.Belt
{
    using global::Belt.Equatable;

    using Xunit;

    public class MemberwiseEquatableTest
    {

        [Fact]
        public void Self_equality_is_sane()
        {
            var x = new ValueObject("asdf", 42, true, new CustomRefType("qwer"));

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
            var x = new ValueObject("asdf", 42, true, customRefType);
            var y = new ValueObject("asdf", 42, false, customRefType);

            Assert.True(x.Equals(y));
            Assert.True(y.Equals(x));
            Assert.True(x == y);
            Assert.False(x != y);

            Assert.Equal(x.GetHashCode(), y.GetHashCode());
        }

        [Fact]
        public void Contained_value_types_are_compared_by_reference()
        {
            var x = new ValueObject("asdf", 42, true, new CustomRefType("qwer"));
            var y = new ValueObject("asdf", 42, true, new CustomRefType("qwer")); // The second CustomRefType is different from the first

            Assert.False(x.Equals(y));
            Assert.False(y.Equals(x));
            Assert.False(x == y);
            Assert.True(x != y);

            Assert.NotEqual(x.GetHashCode(), y.GetHashCode());
        }

        // TODO Add sequence equality tests

        private class ValueObject : MemberwiseEquatable<ValueObject>
        {
            private readonly string _value1;

            private readonly int _value2;

            [MemberwiseEqualityIgnore]
            private readonly bool _value3;

            private readonly CustomRefType _value4;

            public ValueObject(string value1, int value2, bool value3, CustomRefType value4)
            {
                _value1 = value1;
                _value2 = value2;
                _value3 = value3;
                _value4 = value4;
            }

            public string Value1 { get { return _value1; } }

            public int Value2 { get { return _value2; } }

            public bool Value3 { get { return _value3; } }

            public CustomRefType Value4 { get { return _value4; } }
        }

        private class CustomRefType
        {
            private readonly string _s;

            public CustomRefType(string s)
            {
                _s = s;
            }

            public string S { get { return _s; } }
        }
    }
}
