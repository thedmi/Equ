namespace BeltTest.Belt.Serialization.JsonNet
{
    using System.Diagnostics;
    using System.Dynamic;

    using global::Belt.Maybe;
    using global::Belt.Serialization.JsonNet;

    using Newtonsoft.Json;

    using Xunit;

    public class MaybeConverterTest
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings().ConfigureForBelt();

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

        [Fact]
        public void Empty_nested_maybe_roundtrips_successfully_to_json()
        {
            var testObj = new NestedType<InnerType> { TheMaybe = Maybe.Empty<InnerType>() };

            var deserialized = PerformRoundtrip(testObj);

            Assert.True(deserialized.TheMaybe.IsEmpty);
        }

        [Fact]
        public void Existing_nested_maybe_roundtrips_successfully_to_json()
        {
            var testObj = new NestedType<InnerType> { TheMaybe = Maybe.Is(new InnerType { IntValue = 42, StringValue = "Heyo" }) };

            var deserialized = PerformRoundtrip(testObj);

            Assert.True(deserialized.TheMaybe.Exists);
            Assert.Equal(42, deserialized.TheMaybe.It.IntValue);
            Assert.Equal("Heyo", deserialized.TheMaybe.It.StringValue);
        }

        [Fact]
        public void Empty_array_maybe_roundtrips_successfully_to_json()
        {
            var testObj = new NestedType<int[]> { TheMaybe = Maybe.Empty<int[]>() };

            var deserialized = PerformRoundtrip(testObj);

            Assert.True(deserialized.TheMaybe.IsEmpty);
        }

        [Fact]
        public void Existing_array_maybe_roundtrips_successfully_to_json()
        {
            var testObj = new NestedType<int[]> { TheMaybe = Maybe.Is(new []{ 42, 43 }) };

            var deserialized = PerformRoundtrip(testObj);

            Assert.True(deserialized.TheMaybe.Exists);
            Assert.Equal(2, deserialized.TheMaybe.It.Length);
            Assert.Equal(42, deserialized.TheMaybe.It[0]);
            Assert.Equal(43, deserialized.TheMaybe.It[1]);
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

        private class NestedType<T>
        {
            public IMaybe<T> TheMaybe { get; set; } 
        }

        private class InnerType
        {
            public int IntValue { get; set; }
            public string StringValue { get; set; }
        }
    }
}
