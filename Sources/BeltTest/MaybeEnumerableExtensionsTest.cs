// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MaybeEnumerableExtensionsTest.cs" company="Dani Michel">
//   Dani Michel 2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BeltTest
{
    using System;
    using System.Collections.Generic;

    using Belt.FinalList;
    using Belt.Maybe;

    using Xunit;

    public class MaybeEnumerableExtensionsTest
    {
        [Fact]
        public void FirstAsMaybeReturnsEmptyForEmptyList()
        {
            var result = new List<int>().FirstAsMaybe();
            Assert.True(result.IsEmpty);
        }

        [Fact]
        public void FirstAsMaybeWithPredicateReturnsEmptyForEmptyList()
        {
            var result = new List<int> { 2, 4 }.FirstAsMaybe(IsOdd);
            Assert.True(result.IsEmpty);
        }

        [Fact]
        public void FirstAsMaybeReturnsTheFirstValueWhenTheInputIsNonempty()
        {
            var result = new List<int> { 2, 4 }.FirstAsMaybe();
            Assert.Equal(2, result.It);
        }

        [Fact]
        public void FirstAsMaybeWithPredicateReturnsTheFirstMatchWhenThereIsOne()
        {
            var result = new List<int> { 2, 3, 5 }.FirstAsMaybe(IsOdd);
            Assert.Equal(3, result.It);
        }

        [Fact]
        public void SingleAsMaybeReturnsEmptyForEmptyList()
        {
            var result = new List<int>().SingleAsMaybe();
            Assert.True(result.IsEmpty);
        }

        [Fact]
        public void SingleAsMaybeReturnsTheSingleValueWhenTheInputIsNonempty()
        {
            var result = new List<int> { 7 }.SingleAsMaybe();
            Assert.Equal(7, result.It);
        }

        [Fact]
        public void SingleAsMaybeThrowsWhenTheInputIsLargerThanOneElement()
        {
            var list = new List<int> { 4, 7 };
            Assert.Throws<InvalidOperationException>(() => list.SingleAsMaybe());
        }

        [Fact]
        public void SingleAsMaybeWithPredicateReturnsEmptyForNoMatch()
        {
            var result = new List<int> { 2, 4, 6 }.SingleAsMaybe(IsOdd);
            Assert.True(result.IsEmpty);
        }

        [Fact]
        public void SingleAsMaybeWithPredicateReturnsTheSingleValueThatMatches()
        {
            var result = new List<int> { 2, 3, 4, 6 }.SingleAsMaybe(IsOdd);
            Assert.Equal(3, result.It);
        }

        [Fact]
        public void SingleAsMaybeThrowsWhenTheThereIsMoreThanOneMatch()
        {
            var list = new List<int> { 4, 7, 9 };
            Assert.Throws<InvalidOperationException>(() => list.SingleAsMaybe(IsOdd));
        }

        [Fact]
        public void ExistingOnlyFiltersOutNonexistentElements()
        {
            var input = FinalList.Create(Maybe.Is(2), Maybe.Is(5), Maybe.Empty<int>(), Maybe.Is(1));
            var result = input.ExistingOnly();

            Assert.Equal(new[] { 2, 5, 1 }, result);
        }

        private static bool IsOdd(int i)
        {
            return i % 2 == 1;
        }
    }
}
