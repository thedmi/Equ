// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Maybe.cs" company="Dani Michel">
//   Dani Michel 2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Belt.Maybe
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;

    using Belt.FinalList;

    public static class Maybe
    {
        /// <summary>
        /// Creates an <c>IMaybe</c> from a value.
        /// </summary>
        /// <param name="value">The value of the maybe. Must not be null</param>
        [Pure]
        [DebuggerStepThrough]
        public static Maybe<T> Is<T>(T value)
        {
            return new Maybe<T>(value);
        }

        /// <summary>
        /// Creates an empty <c>IMaybe</c>.
        /// </summary>
        [Pure]
        public static Maybe<T> Empty<T>()
        {
            return new Maybe<T>();
        }

        /// <summary>
        /// Creates an <c>IMaybe</c> from a value if it is not null.
        /// Else it creates an empty <c>IMaybe</c>. 
        /// </summary>
        [Pure]
        [DebuggerStepThrough]
        public static Maybe<T> FromNullable<T>(T value) where T : class
        {
            return value != null ? Is(value) : Empty<T>();
        }

        /// <summary>
        /// Creates an <c>IMaybe</c> from a value if it is not null.
        /// Else it creates an empty <c>IMaybe</c>. 
        /// </summary>
        [Pure]
        [DebuggerStepThrough]
        public static Maybe<T> FromNullable<T>(T? value) where T : struct
        {
            return value.HasValue ? Is(value.Value) : Empty<T>();
        }

        /// <summary>
        /// Use the <paramref name="valueProducer"/> to try and produce an instance of 
        /// <typeparamref name="T"/>. If that works, returns an initialized <c>IMaybe</c>,
        /// or an empty one otherwise.
        /// Use <see cref="IMaybeTryCatch{T}.Catch{TException}"/> to specify the expected
        /// exception.
        /// </summary>
        [Pure]
        public static IMaybeTryCatch<T> Try<T>(Func<T> valueProducer) 
        {
            return new TryCatch<T>(valueProducer);
        }

        private sealed class TryCatch<T> : IMaybeTryCatch<T> 
        {
            private readonly Func<T> _valueProducer;

            internal TryCatch(Func<T> valueProducer)
            {
                _valueProducer = valueProducer;
            }

            public IMaybe<T> Catch<TException>() where TException : Exception
            {
                try
                {
                    return Is(_valueProducer());
                }
                catch (TException)
                {
                    return Empty<T>();
                }
            }
        }
    }

    public struct Maybe<T> : IMaybe<T>
    {
        private readonly bool _exists;

        private readonly T _value;

        public Maybe(T existingValue)
        {
            // If value is a value type, the comparison will always yield false, see
            // http://stackoverflow.com/a/5340850/219187
            // ReSharper disable CompareNonConstrainedGenericWithNull
            if (existingValue == null)
            // ReSharper restore CompareNonConstrainedGenericWithNull
            {
                throw new ArgumentNullException("existingValue");
            }

            _exists = true;
            _value = existingValue;
        }

        public bool IsEmpty { get { return !_exists; } }

        public bool Exists { get { return _exists; } }

        public T It
        {
            get
            {
                if (_exists)
                {
                    return _value;
                }
                throw new InvalidOperationException("Maybe-value is null.");
            }
        }

        public T ItOrDefault { get { return _exists ? _value : default(T); } }

        public T ItOrThrow(Exception exception)
        {
            if (_exists)
            {
                return _value;
            }

            throw exception;
        }

        public T ItOrThrow(Func<Exception> exceptionCreator)
        {
            if (_exists)
            {
                return _value;
            }

            throw exceptionCreator();
        }

        public IFinalList<T> AsList()
        {
            return _exists ? FinalList.Create(_value) : FinalList.Empty<T>();
        }

        public override int GetHashCode()
        {
            return _exists ? _value.GetHashCode() : 7907;
        }
    }
}
