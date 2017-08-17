namespace EquTest
{
    using Equ;

    using Xunit;

    public class PropertywiseEquatableTest
    {
        [Fact]
        public void Self_equality_is_sane()
        {
            var x = new ValueType("asdf", 42);

#pragma warning disable 1718
            // ReSharper disable EqualExpressionComparison
            Assert.True(x.Equals(x));
            Assert.True(x == x);
            Assert.False(x != x);
            Assert.False(x.Equals(null));
            Assert.False(Equals(null, x));

            Assert.Equal(x.GetHashCode(), x.GetHashCode());
            // ReSharper restore EqualExpressionComparison
#pragma warning restore 1718
        }

        [Fact]
        public void Value_objects_with_equal_members_are_equal()
        {
            var x = new ValueType("asdf", 42);
            var y = new ValueType("asdf", 42);

            Assert.True(x.Equals(y));
            Assert.True(y.Equals(x));
            Assert.True(x == y);
            Assert.False(x != y);

            Assert.Equal(x.GetHashCode(), y.GetHashCode());
        }

        [Fact]
        public void Value_objects_with_one_differing_member_are_unequal()
        {
            var x = new ValueType("asdf", 100);
            var y = new ValueType("asdf", 42);

            Assert.False(x.Equals(y));
            Assert.False(y.Equals(x));
            Assert.False(x == y);
            Assert.True(x != y);

            Assert.NotEqual(x.GetHashCode(), y.GetHashCode());
        }

        private class ValueType : PropertywiseEquatable<ValueType>
        {
            public ValueType(string x, int y)
            {
                X = x;
                Y = y;
            }

            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string X { get; private set; }

            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public int Y { get; private set; }
        }
    }
}
