using System.Collections.Generic;
using Xunit;

namespace Equ.Test
{
    public class NestedClassTest
    {
        public class Nested
        {
            public string BasicProperty { get; set; }
        }

        public class Container
        {
            public Nested Nested { get; set; }
        }

        public static IEnumerable<object[]> ShouldDetermineCorrectEqualityTests => new List<object[]>
        {
            new object[] { new Container(), new Container(), MemberwiseEqualityComparer<Container>.ByProperties, true },
            new object[] {
                new Container { Nested = new Nested() },
                new Container { },
                MemberwiseEqualityComparer<Container>.ByProperties,
                false
            },
            new object[] {
                new Container { Nested = new Nested() },
                new Container { Nested = new Nested() },
                MemberwiseEqualityComparer<Container>.ByProperties,
                false
            },
            new object[] {
                new Container { Nested = new Nested() },
                new Container { Nested = new Nested() },
                MemberwiseEqualityComparer<Container>.ByPropertiesRecursive,
                true
            }
        };

        [Theory]
        [MemberData(nameof(ShouldDetermineCorrectEqualityTests))]
        public void ShouldDetermineCorrectEquality(Container x, Container y, MemberwiseEqualityComparer<Container> equalityComparer, bool expected)
        {
            Assert.Equal(expected, equalityComparer.Equals(x, y));
        }
    }
}
