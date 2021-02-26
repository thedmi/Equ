using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Equ.Test
{
    public class NestedCollectionsTest
    {
        public class Level2
        {
            public string BasicProperty { get; set; }
        }

        public class Level1
        {
            public ICollection<Level2> Items { get; set; }
        }

        public class Container
        {
            public ICollection<Level1> Items { get; set; }
        }

        public static IEnumerable<object[]> ShouldDetermineCorrectEqualityTests => new List<object[]>
        {
            new object[] { new Container(), new Container(), MemberwiseEqualityComparer<Container>.ByProperties, true },
            new object[] {
                new Container { Items = new List<Level1>() },
                new Container { },
                MemberwiseEqualityComparer<Container>.ByProperties,
                false
            },
            new object[] {
                new Container { Items = new List<Level1>() },
                new Container { Items = new List<Level1>() },
                MemberwiseEqualityComparer<Container>.ByProperties,
                true
            },
            new object[] {
                new Container { Items = new List<Level1>() },
                new Container { Items = Array.Empty<Level1>() },
                MemberwiseEqualityComparer<Container>.ByProperties,
                true
            },
            new object[] {
                new Container { Items = new [] { new Level1() } },
                new Container { Items = Array.Empty<Level1>() },
                MemberwiseEqualityComparer<Container>.ByProperties,
                false
            },
            new object[] {
                new Container { Items = new [] { new Level1() } },
                new Container { Items = new [] { new Level1() } },
                MemberwiseEqualityComparer<Container>.ByProperties,
                false
            },
            new object[] {
                new Container { Items = new [] { new Level1() } },
                new Container { Items = new [] { new Level1() } },
                MemberwiseEqualityComparer<Container>.ByPropertiesRecursive,
                true
            },
            new object[] {
                new Container { Items = new [] { new Level1 { Items = new[] { new Level2() } } } },
                new Container { Items = new [] { new Level1 { Items = new[] { new Level2() } } } },
                MemberwiseEqualityComparer<Container>.ByProperties,
                false
            },
            new object[] {
                new Container { Items = new [] { new Level1 { Items = new[] { new Level2() } } } },
                new Container { Items = new [] { new Level1 { Items = new[] { new Level2() } } } },
                MemberwiseEqualityComparer<Container>.ByPropertiesRecursive,
                true
            },
        };

        [Theory]
        [MemberData(nameof(ShouldDetermineCorrectEqualityTests))]
        public void ShouldDetermineCorrectEquality(Container x, Container y, MemberwiseEqualityComparer<Container> equalityComparer, bool expected)
        {
            Assert.Equal(expected, equalityComparer.Equals(x, y));
        }
    }
}
