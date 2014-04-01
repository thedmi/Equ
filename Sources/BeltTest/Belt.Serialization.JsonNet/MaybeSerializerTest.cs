namespace BeltTest.Belt.Serialization.JsonNet
{
    using System.Diagnostics;

    using global::Belt.Maybe;
    using global::Belt.Serialization.JsonNet;

    using Newtonsoft.Json;

    using Xunit;

    public class MaybeSerializerTest
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            Converters = new JsonConverter[] { new MaybeConverter() }
        };

        [Fact]
        public void Empty_maybe_roundtrips_successfully_to_json()
        {
            var testObj = new TestRefType { TheMaybe = Maybe.Empty<string>() };

            var deserialized = PerformRoundtrip(testObj);

            Assert.True(deserialized.TheMaybe.IsEmpty);
        }

        [Fact]
        public void Existing_maybe_roundtrips_successfully_to_json()
        {
            var testObj = new TestRefType { TheMaybe = Maybe.Is("Hello maybe!") };

            var deserialized = PerformRoundtrip(testObj);

            Assert.True(deserialized.TheMaybe.Exists);
            Assert.Equal("Hello maybe!", deserialized.TheMaybe.It);
        }

        [Fact]
        public void Empty_value_maybe_roundtrips_successfully_to_json()
        {
            var testObj = new TestValueType { TheMaybe = Maybe.Empty<int>() };

            var deserialized = PerformRoundtrip(testObj);

            Assert.True(deserialized.TheMaybe.IsEmpty);
        }

        [Fact]
        public void Existing_value_maybe_roundtrips_successfully_to_json()
        {
            var testObj = new TestValueType { TheMaybe = Maybe.Is(42) };

            var deserialized = PerformRoundtrip(testObj);

            Assert.True(deserialized.TheMaybe.Exists);
            Assert.Equal(42, deserialized.TheMaybe.It);
        }

        private T PerformRoundtrip<T>(T testObj)
        {
            var json = JsonConvert.SerializeObject(testObj, _jsonSerializerSettings);

            Debug.WriteLine(json);

            return JsonConvert.DeserializeObject<T>(json, _jsonSerializerSettings);
        }

        private class TestRefType
        {
            public IMaybe<string> TheMaybe { get; set; }
        }

        private class TestValueType
        {
            public IMaybe<int> TheMaybe { get; set; }
        }
    }
}
