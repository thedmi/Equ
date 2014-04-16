namespace BeltTest.Belt.Serialization.JsonNet
{
    using System.Diagnostics;

    using global::Belt.FinalList;
    using global::Belt.Maybe;
    using global::Belt.Serialization.JsonNet;

    using Newtonsoft.Json;

    using Xunit;

    public class ComplexObjectConversionTest
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings().ConfigureForBelt();

        [Fact]
        public void Complex_object_roundtrips_to_json()
        {
            var obj =
                new Wrapper(
                    new Outer(
                        Maybe.Empty<IntWithString>(),
                        FinalList.Create(
                            new Outer(Maybe.Is(new IntWithString(42, "hello")), FinalList.Empty<Outer>()),
                            new Outer(Maybe.Empty<IntWithString>(), FinalList.Empty<Outer>()))));

            var deserialized = PerformRoundtrip(obj);

            Assert.NotNull(deserialized);
            Assert.True(deserialized.Outer.Maybe.IsEmpty);
            Assert.Equal(2, deserialized.Outer.Outers.Count);
            Assert.Equal(42, deserialized.Outer.Outers[0].Maybe.It.V1);
            Assert.Equal("hello", deserialized.Outer.Outers[0].Maybe.It.V2);
            Assert.True(deserialized.Outer.Outers[1].Maybe.IsEmpty);
        }

        private T PerformRoundtrip<T>(T testObj)
        {
            var json = JsonConvert.SerializeObject(testObj, _jsonSerializerSettings);

            Debug.WriteLine(json);

            return JsonConvert.DeserializeObject<T>(json, _jsonSerializerSettings);
        }

        private class Wrapper
        {
            private readonly Outer _outer;

            public Wrapper(Outer outer)
            {
                _outer = outer;
            }

            public Outer Outer { get { return _outer; } }
        }

        private class Outer
        {
            private readonly IMaybe<IntWithString> _maybe;

            private readonly IFinalList<Outer> _outers;

            public Outer(IMaybe<IntWithString> maybe, IFinalList<Outer> outers)
            {
                _maybe = maybe;
                _outers = outers;
            }

            public IMaybe<IntWithString> Maybe { get { return _maybe; } }

            public IFinalList<Outer> Outers { get { return _outers; } }
        }

        private class IntWithString
        {
            private readonly int _v1;

            private readonly string _v2;

            public IntWithString(int v1, string v2)
            {
                _v1 = v1;
                _v2 = v2;
            }

            public int V1 { get { return _v1; } }

            public string V2 { get { return _v2; } }
        }
    }
}
