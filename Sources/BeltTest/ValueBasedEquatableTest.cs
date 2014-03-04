// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FinalList.cs" company="Dani Michel">
//   Dani Michel 2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BeltTest
{
    using Belt.Equatable;

    using Xunit;

    public class ValueBasedEquatableTest
    {
        [Fact]
        public void SelfEqualityIsSane()
        {
            var x = new TestValue(6);

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
        public void EqualityIsBasedOnValue()
        {
            var x = new TestValue(3);
            var y = new TestValue(3);

            Assert.True(x.Equals(y));
            Assert.True(y.Equals(x));
            Assert.True(x == y);
            Assert.False(x != y);

            Assert.Equal(x.GetHashCode(), y.GetHashCode());
        }

        private class TestValue : ValueBasedEquatable<TestValue, int>
        {
            private readonly int _value;

            public TestValue(int value)
            {
                _value = value;
            }

            protected override int EquatableValue { get { return _value; } }
        }
    }
}
