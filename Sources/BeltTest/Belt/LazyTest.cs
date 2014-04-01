// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LazyTest.cs" company="Dani Michel">
//   Dani Michel 2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BeltTest.Belt
{
    using global::Belt.Lazy;

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
                    return 42;
                });

            Assert.False(evaluated);

            Assert.Equal(42, lazy.Value);

            Assert.True(evaluated);
        }
    }
}
