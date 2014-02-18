// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FinalListAdapterTest.cs" company="Dani Michel">
//   Dani Michel 2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BeltTest
{
    using System.Collections.Immutable;

    using Belt.FinalList;

    using Xunit;

    public class FinalListAdapterTest
    {
        [Fact]
        public void CreateMaintainsOrder()
        {
            var list = FinalList.Create("one", "two", "three");

            Assert.Equal("one", list[0]);
            Assert.Equal("two", list[1]);
            Assert.Equal("three", list[2]);
        }

        [Fact]
        public void CreateMaintainsListSize()
        {
            var list = FinalList.Create(1, 2, 3);

            Assert.False(list.IsEmpty);
            Assert.Equal(3, list.Count);
        }

        [Fact]
        public void EmptyListIsOfSize0()
        {
            var list = FinalList.Empty<int>();

            Assert.True(list.IsEmpty);
            Assert.Equal(0, list.Count);
        }

        [Fact]
        public void ConstructionFromImmutableListDoesNotCreateCopy()
        {
            var innerList = ImmutableList.Create(1, 2);
            var list = new FinalListAdapter<int>(innerList);

            var effectiveInnerList = list.ImmutableList;

            Assert.True(ReferenceEquals(innerList, effectiveInnerList));
        }
    }
}
