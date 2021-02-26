using System.Collections.Generic;
using Xunit;

namespace Equ.Test
{
    public class NestedClassTest
    {
        public class Contained
        {
            public string BasicProperty { get; set; }
            public Contained Nested { get; set; }
        }

        public class Container
        {
            public Contained Nested { get; set; }
        }

        public static IEnumerable<object[]> ShouldDetermineCorrectEqualityTests => new List<object[]>
        {
            new object[] { new Container(), new Container(), MemberwiseEqualityComparer<Container>.ByProperties, true },
            new object[] {
                new Container { Nested = new Contained() },
                new Container { },
                MemberwiseEqualityComparer<Container>.ByProperties,
                false
            },
            new object[] {
                new Container { Nested = new Contained() },
                new Container { Nested = new Contained() },
                MemberwiseEqualityComparer<Container>.ByProperties,
                false
            },
            new object[] {
                new Container { Nested = new Contained() },
                new Container { Nested = new Contained() },
                MemberwiseEqualityComparer<Container>.ByPropertiesRecursive,
                true
            },
            new object[] {
                new Container { Nested = new Contained { Nested = new Contained() } },
                new Container { Nested = new Contained() },
                MemberwiseEqualityComparer<Container>.ByPropertiesRecursive,
                false
            },
            new object[] {
                new Container { Nested = new Contained { Nested = new Contained() } },
                new Container { Nested = new Contained { Nested = new Contained() } },
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
