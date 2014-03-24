// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LazyTest.cs" company="Dani Michel">
//   Dani Michel 2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BeltTest
{
    using Belt.Lazy;

    using Xunit;

    public class LazyTest
    {
        [Fact]
        public void LazyDoesEvaluateLazily()
        {
            var evaluated = false;

            var lazy = Lazy.Create(
                () =>
                {
                    // Close over 'evaluated' for the sake of the test
                    // (don't do this in production)
                    evaluated = true;
                    return 0;
                });

            Assert.False(evaluated);

            Assert.NotNull(lazy.Value);

            Assert.True(evaluated);
        }
    }
}
