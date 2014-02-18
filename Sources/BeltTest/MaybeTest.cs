// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MaybeTest.cs" company="Dani Michel">
//   Dani Michel 2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BeltTest
{
    using System;

    using Belt.Maybe;

    using Xunit;

    public class MaybeTest
    {
        [Fact]
        public void EmptyMaybeWorksForValueTypes()
        {
            var maybe = Maybe.Empty<int>();

            Assert.True(maybe.IsEmpty);
            Assert.False(maybe.Exists);
            Assert.Equal(0, maybe.ItOrDefault);
            Assert.Throws<InvalidOperationException>(() => maybe.It);
            Assert.Throws<InvalidTimeZoneException>(() => maybe.ItOrThrow(new InvalidTimeZoneException()));
            Assert.Equal(0, maybe.AsList().Count);
        }

        [Fact]
        public void EmptyMaybeWorksForReferenceTypes()
        {
            var maybe = Maybe.Empty<string>();

            Assert.True(maybe.IsEmpty);
            Assert.False(maybe.Exists);
            Assert.Null(maybe.ItOrDefault);
            Assert.Throws<InvalidOperationException>(() => maybe.It);
            Assert.Throws<InvalidTimeZoneException>(() => maybe.ItOrThrow(new InvalidTimeZoneException()));
            Assert.Equal(0, maybe.AsList().Count);
        }

        [Fact]
        public void ExistingMaybeWorksForValueTypes()
        {
            var maybe = Maybe.Is(34.2);

            Assert.False(maybe.IsEmpty);
            Assert.True(maybe.Exists);
            Assert.Equal(34.2, maybe.It);
            Assert.Equal(34.2, maybe.ItOrDefault);
            Assert.Equal(34.2, maybe.ItOrThrow(new InvalidTimeZoneException()));
            Assert.Equal(new[] { 34.2 }, maybe.AsList());
        }

        [Fact]
        public void ExistingMaybeWorksForReferenceTypes()
        {
            var maybe = Maybe.Is("Hello");

            Assert.False(maybe.IsEmpty);
            Assert.True(maybe.Exists);
            Assert.Equal("Hello", maybe.It);
            Assert.Equal("Hello", maybe.ItOrDefault);
            Assert.Equal("Hello", maybe.ItOrThrow(new InvalidTimeZoneException()));
            Assert.Equal(new[] { "Hello" }, maybe.AsList());
        }
    }
}
