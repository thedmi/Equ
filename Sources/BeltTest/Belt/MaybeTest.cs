// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MaybeTest.cs" company="Dani Michel">
//   Dani Michel 2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BeltTest.Belt
{
    using System;

    using global::Belt.Maybe;

    using Xunit;

#pragma warning disable 612
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
            Assert.Throws<InvalidTimeZoneException>(() => maybe.ItOrThrow(() => new InvalidTimeZoneException()));
            Assert.Null(maybe.AsNullable());
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
            Assert.Throws<InvalidTimeZoneException>(() => maybe.ItOrThrow(() => new InvalidTimeZoneException()));
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
            Assert.Equal(34.2, maybe.ItOrThrow(() => new InvalidTimeZoneException()));
            Assert.Equal(34.2, maybe.AsNullable());
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
            Assert.Equal("Hello", maybe.ItOrThrow(() => new InvalidTimeZoneException()));
            Assert.Equal(new[] { "Hello" }, maybe.AsList());
        }

        [Fact]
        public void CanConstructMaybeFromNullable()
        {
            var existing = Maybe.FromNullable("Hello");
            Assert.Equal("Hello", existing.It);

            var emptyRef = Maybe.FromNullable((string)null);
            Assert.True(emptyRef.IsEmpty);
            Assert.Null(emptyRef.ItOrDefault);

            var emptyStruct = Maybe.FromNullable((int?)null);
            Assert.True(emptyStruct.IsEmpty);
            Assert.Equal(0, emptyStruct.ItOrDefault);
        }

        [Fact]
        public void SelectMaintainsTheExistsOrEmptyState()
        {
            var lengthMaybe = Maybe.Is("Hello").Select(s => s.Length);

            Assert.True(lengthMaybe.Exists);
            Assert.Equal(5, lengthMaybe.It);

            var emptyLengthMaybe = Maybe.Empty<string>().Select(s => s.Length);

            Assert.False(emptyLengthMaybe.Exists);
        }
    }
#pragma warning restore 612
}
