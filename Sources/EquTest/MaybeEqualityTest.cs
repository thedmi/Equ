// ReSharper disable NotAccessedField.Local
namespace EquTest
{
    using Equ;

    using MayBee;

    using Xunit;

    public class MaybeEqualityTest
    {
        [Fact]
        public void Equality_on_maybe_propagates_to_inner_value()
        {
            var v1 = new SomeValue1(Maybe.Is(new SomeValue2("asdf")));
            var v2 = new SomeValue1(Maybe.Is(new SomeValue2("asdf")));

            Assert.Equal(v1, v2);
        }

        [Fact]
        public void Empty_values_are_considered_equal()
        {
            var v1 = new SomeValue1(Maybe.Empty<SomeValue2>());
            var v2 = new SomeValue1(Maybe.Empty<SomeValue2>());

            Assert.Equal(v1, v2);
        }

        [Fact]
        public void Empty_values_are_not_equal_to_existing()
        {
            var v1 = new SomeValue1(Maybe.Empty<SomeValue2>());
            var v2 = new SomeValue1(Maybe.Is(new SomeValue2("asdf")));

            Assert.NotEqual(v1, v2);
        }

        private class SomeValue1 : MemberwiseEquatable<SomeValue1>
        {
            private readonly Maybe<SomeValue2> _theValue;

            public SomeValue1(Maybe<SomeValue2> theValue)
            {
                _theValue = theValue;
            }
        }

        private class SomeValue2 : MemberwiseEquatable<SomeValue2>
        {
            private readonly string _val;

            public SomeValue2(string val)
            {
                _val = val;
            }
        }
    }
}
