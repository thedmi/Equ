// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FinalList.cs" company="Dani Michel">
//   Dani Michel 2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BeltTest.Belt
{
    using System.Collections.Generic;

    using global::Belt.Equatable;

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
            Assert.Equal(3.GetHashCode(), new ReferenceValue(new TestValue(3)).GetHashCode());
            Assert.Equal(0, new ReferenceValue(null).GetHashCode());
        }

        [Fact]
        public void EqualityBasedOnSequenceValuesComparesByElement()
        {
            var seq1 = new TestSequenceValue(new List<TestValue> { new TestValue(1), new TestValue(2) });
            var seq2 = new TestSequenceValue(new [] { new TestValue(1), new TestValue(2) });

            Assert.Equal(seq1, seq2);
            Assert.True(seq1 == seq2);
        }

        [Fact]
        public void EqualityBasedSequencesConvertToStringAsArray()
        {
            var seq = new TestSequenceValue(new[] { new TestValue(1), new TestValue(2) });

            Assert.Equal("[ 1, 2 ]", seq.ToString());
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

        private class ReferenceValue : ValueBasedEquatable<ReferenceValue, TestValue>
        {
            private readonly TestValue _value;

            public ReferenceValue(TestValue value)
            {
                _value = value;
            }

            protected override TestValue EquatableValue { get { return _value; } }
        }

        private class TestSequenceValue : ValueBasedEquatable<TestSequenceValue, EquatableSequenceWrapper<TestValue>>
        {
            private readonly IEnumerable<TestValue> _values;

            public TestSequenceValue(IEnumerable<TestValue> values)
            {
                _values = values;
            }

            protected override EquatableSequenceWrapper<TestValue> EquatableValue { get { return _values.AsEquatableSequence(); } }
        }
    }
}
