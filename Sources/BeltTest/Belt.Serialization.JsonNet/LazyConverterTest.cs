namespace BeltTest.Belt.Serialization.JsonNet
{
    using System.Diagnostics;

    using global::Belt.Lazy;
    using global::Belt.Serialization.JsonNet;

    using Newtonsoft.Json;

    using Xunit;

    public class LazyConverterTest
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            Converters = new JsonConverter[] { new LazyConverter() }
        };

        [Fact]
        public void Can_roundtrip_value_typed_lazy()
        {
            var testObj = new Test<int> { TheLazy = Lazy.Create(() => 42) };

            var deserialized = Roundtrip(testObj);

            Assert.Equal(42, deserialized.TheLazy.Value);
        }

        [Fact]
        public void Can_roundtrip_reference_typed_lazy()
        {
            var testObj = new Test<string> { TheLazy = Lazy.Create(() => "Hello") };

            var deserialized = Roundtrip(testObj);

            Assert.Equal("Hello", deserialized.TheLazy.Value);
        }

        [Fact]
        public void Can_roundtrip_null_lazy()
        {
            var testObj = new Test<string> { TheLazy = Lazy.Create<string>(() => null) };

            var deserialized = Roundtrip(testObj);

            Assert.Equal(null, deserialized.TheLazy.Value);
        }

        private T Roundtrip<T>(T testObj)
        {
            var json = JsonConvert.SerializeObject(testObj, _jsonSerializerSettings);

            Debug.WriteLine(json);

            return JsonConvert.DeserializeObject<T>(json, _jsonSerializerSettings);
        }

        private class Test<T>
        {
            public ILazy<T> TheLazy { get; set; }
        }
    }
}
