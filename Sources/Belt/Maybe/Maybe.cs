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
        public static IMaybe<T> Is<T>(T value)
        {
            return new Impl.Existing<T>(value);
        }

        /// <summary>
        /// Creates an empty <c>IMaybe</c>.
        /// </summary>
        [Pure]
        public static IMaybe<T> Empty<T>()
        {
            return new Impl.Empty<T>();
        }

        /// <summary>
        /// Creates an <c>IMaybe</c> from a value if it is not null.
        /// Else it creates an empty <c>IMaybe</c>. 
        /// </summary>
        [Pure]
        [DebuggerStepThrough]
        public static IMaybe<T> FromNullable<T>(T value) where T : class
        {
            return value != null ? Is(value) : Empty<T>();
        }

        /// <summary>
        /// Creates an <c>IMaybe</c> from a value if it is not null.
        /// Else it creates an empty <c>IMaybe</c>. 
        /// </summary>
        [Pure]
        [DebuggerStepThrough]
        public static IMaybe<T> FromNullable<T>(T? value) where T : struct
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

        private static class Impl
        {
            [DebuggerStepThrough]
            internal sealed class Existing<T> : IMaybe<T>
            {
                private readonly T _value;

                public Existing(T value)
                {
                    // If value is a value type, the comparison will always yield false, see
                    // http://stackoverflow.com/a/5340850/219187
                    // ReSharper disable CompareNonConstrainedGenericWithNull
                    if (value == null)
                    // ReSharper restore CompareNonConstrainedGenericWithNull
                    {
                        throw new ArgumentNullException("value");
                    }

                    _value = value;
                }

                public T It
                {
                    get { return _value; }
                }

                public T ItOrDefault { get { return _value; } }
                
                public bool IsEmpty { get { return false; } }

                public bool Exists { get { return true; } }

                public T ItOrThrow(Exception exception)
                {
                    return It;
                }
                public T ItOrThrow(Func<Exception> exceptionCreator)
                {
                    return It;
                }

                public IFinalList<T> AsList()
                {
                    return FinalList.Create(_value);
                }

                private bool Equals(Existing<T> other)
                {
                    return Equals(_value, other._value);
                }

                public override bool Equals(object obj)
                {
                    if (ReferenceEquals(null, obj))
                    {
                        return false;
                    }
                    if (ReferenceEquals(this, obj))
                    {
                        return true;
                    }
                    return obj is Existing<T> && Equals((Existing<T>)obj);
                }

                public override int GetHashCode()
                {
                    return _value.GetHashCode();
                }

                public override string ToString()
                {
                    return string.Format("Existing {0}", _value);
                }
            }

            [DebuggerStepThrough]
            // ReSharper disable MemberHidesStaticFromOuterClass
            internal sealed class Empty<T> : IMaybe<T> 
            // ReSharper restore MemberHidesStaticFromOuterClass
            {
                public T It { get { throw new InvalidOperationException("Maybe-value is null."); } }

                public T ItOrDefault { get { return default(T); } }

                public bool IsEmpty { get { return true; } }

                public bool Exists { get { return false; } }

                public T ItOrThrow(Exception exception)
                {
                    throw exception;
                }

                public T ItOrThrow(Func<Exception> exceptionCreator)
                {
                    throw exceptionCreator();
                }

                public IFinalList<T> AsList()
                {
                    return FinalList.Empty<T>();
                }

                public override bool Equals(object obj)
                {
                    if (ReferenceEquals(null, obj))
                    {
                        return false;
                    }
                    return obj is Empty<T>;
                }

                public override int GetHashCode()
                {
                    return 0;
                }

                public override string ToString()
                {
                    return string.Format("Empty");
                }
            }
        }
    }
}
