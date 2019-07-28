namespace Equ.Test
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text.RegularExpressions;

    using Xunit;
    using Xunit.Abstractions;

    public class ToStringFunctionGeneratorTest
    {
        private readonly ITestOutputHelper _output;

        public ToStringFunctionGeneratorTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void String_representations_contain_all_information()
        {
            var obj = new SomeType1(
                12,
                "uga",
                new[] { new SomeType2(true, 42.8), new SomeType2(false, double.NaN) },
                new[] { 1, 2, 3, 4 },
                new Dictionary<string, SomeType2> { { "foo", new SomeType2(false, 13.1) } });

            Assert.Equal(
                "{ _anIntegerArray: [ 1, 2, 3, 4 ], _aDictionary: [ [foo, { SomeBool: False, SomeDouble: 13.1 }] ], SomeInteger: 12, SomeString: uga, SomeType2S: [ { SomeBool: True, SomeDouble: 42.8 }, { SomeBool: False, SomeDouble: NaN } ] }",
                obj.ToString());
        }

        [Fact]
        public void Null_values_are_converted_to_null_strings()
        {
            var obj = new SomeType1(0, null, null, null, null);
            
            Assert.Equal("", obj.ToString());
        }
        
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // ReSharper disable NotAccessedField.Local
        private class SomeType1
        {
            private readonly int[] _anIntegerArray;

            private readonly IDictionary<string, SomeType2> _aDictionary;

            public SomeType1(int someInteger, string someString, IReadOnlyList<SomeType2> someType2S, int[] anIntegerArray, IDictionary<string, SomeType2> aDictionary)
            {
                _anIntegerArray = anIntegerArray;
                _aDictionary = aDictionary;
                
                SomeInteger = someInteger;
                SomeType2S = someType2S;
                SomeString = someString;
            }

            public int SomeInteger { get; }
            
            public string SomeString { get; }
            
            public IReadOnlyList<SomeType2> SomeType2S { get; }

            public override string ToString()
            {
                return Stringify<SomeType1>.With(this);
            }
        }

        private class SomeType2
        {
            public SomeType2(bool someBool, double someDouble)
            {
                SomeBool = someBool;
                SomeDouble = someDouble;
            }

            public bool SomeBool { get; }

            public double SomeDouble { get; }

            public override string ToString() => Stringify<SomeType2>.With(this);
        }
        // ReSharper restore NotAccessedField.Local
        // ReSharper restore UnusedAutoPropertyAccessor.Local
    }
}