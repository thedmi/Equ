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
            //new object[] { new Container(), new Container(), true },
            //new object[] { 
            //    new Container { Items = new List<Level1>() },
            //    new Container { },
            //    false
            //},
            //new object[] {
            //    new Container { Items = new List<Level1>() },
            //    new Container { Items = new List<Level1>() },
            //    true
            //},
            //new object[] {
            //    new Container { Items = new List<Level1>() },
            //    new Container { Items = Array.Empty<Level1>() },
            //    true
            //},
            //new object[] {
            //    new Container { Items = new [] { new Level1() } },
            //    new Container { Items = Array.Empty<Level1>() },
            //    false
            //},
            new object[] {
                new Container { Items = new [] { new Level1() } },
                new Container { Items = new [] { new Level1() } },
                true
            },
        };

        [Theory]
        [MemberData(nameof(ShouldDetermineCorrectEqualityTests))]
        public void ShouldDetermineCorrectEquality(Container x, Container y, bool expected)
        {
            Assert.Equal(expected, MemberwiseEqualityComparer<Container>.ByProperties.Equals(x, y));
        }
    }
}
