// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Guard.cs" company="Dani Michel">
//   Dani Michel 2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Belt.Guard
{
    using System.Diagnostics;

    [DebuggerStepThrough]
    public static class Guard
    {
        [ContractAnnotation("false => halt")]
        public static void Precondition(bool condition)
        {
            ThrowIfViolated(condition, "Precondition violated");
        }

        [ContractAnnotation("condition: false => halt")]
        public static void Precondition(bool condition, string preconditionInfo)
        {
            ThrowIfViolated(condition, string.Format("Precondition violated: {0}", preconditionInfo));
        }

        [ContractAnnotation("false => halt")]
        public static void Postcondition(bool condition)
        {
            ThrowIfViolated(condition, "Postcondition violated");
        }

        [ContractAnnotation("condition: false => halt")]
        public static void Postcondition(bool condition, string postconditionInfo)
        {
            ThrowIfViolated(condition, string.Format("Postcondition violated: {0}", postconditionInfo));
        }

        [ContractAnnotation("false => halt")]
        public static void Assertion(bool condition)
        {
            ThrowIfViolated(condition, "Guard violated");
        }

        [ContractAnnotation("condition: false => halt")]
        public static void Assertion(bool condition, string guardInfo)
        {
            ThrowIfViolated(condition, string.Format("Guard violated: {0}", guardInfo));
        }

        [Conditional("DEBUG")]
        [ContractAnnotation("condition: false => halt")]
        public static void DebugAssertion(bool condition)
        {
            ThrowIfViolated(condition, "Guard violated");
        }

        [Conditional("DEBUG")]
        [ContractAnnotation("condition: false => halt")]
        public static void DebugAssertion(bool condition, string guardInfo)
        {
            ThrowIfViolated(condition, string.Format("Guard violated: {0}", guardInfo));
        }

        // ReSharper disable UnusedParameter.Local
        private static void ThrowIfViolated(bool condition, string exceptionInfo)
        // ReSharper restore UnusedParameter.Local
        {
            if (!condition)
            {
                throw new GuardException(exceptionInfo);
            }
        }
    }
}
