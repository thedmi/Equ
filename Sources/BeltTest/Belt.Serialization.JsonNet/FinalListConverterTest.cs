namespace BeltTest.Belt.Serialization.JsonNet
{
    using System.Diagnostics;

    using global::Belt.FinalList;
    using global::Belt.Serialization.JsonNet;

    using Newtonsoft.Json;

    using Xunit;

    public class FinalListConverterTest
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings().ConfigureForBelt();

        [Fact]
        public void Can_roundtrip_value_typed_final_list_with_content()
        {
            var testObj = new Test<int> { TheList = FinalList.Create(42, 43) };

            var deserialized = Roundtrip(testObj);

            Assert.Equal(2, deserialized.TheList.Count);
            Assert.Equal(42, deserialized.TheList[0]);
            Assert.Equal(43, deserialized.TheList[1]);
        }

        [Fact]
        public void Can_roundtrip_empty_value_typed_final_list()
        {
            var testObj = new Test<int> { TheList = FinalList.Empty<int>() };

            var deserialized = Roundtrip(testObj);

            Assert.Equal(0, deserialized.TheList.Count);
        }

        [Fact]
        public void Can_roundtrip_reference_typed_final_list_with_content()
        {
            var testObj = new Test<string> { TheList = FinalList.Create("Hello", "Belt") };

            var deserialized = Roundtrip(testObj);

            Assert.Equal(2, deserialized.TheList.Count);
            Assert.Equal("Hello", deserialized.TheList[0]);
            Assert.Equal("Belt", deserialized.TheList[1]);
        }

        [Fact]
        public void Can_roundtrip_empty_reference_typed_final_list()
        {
            var testObj = new Test<string> { TheList = FinalList.Empty<string>() };

            var deserialized = Roundtrip(testObj);

            Assert.Equal(0, deserialized.TheList.Count);
        }

        private T Roundtrip<T>(T testObj)
        {
            var json = JsonConvert.SerializeObject(testObj, _jsonSerializerSettings);

            Debug.WriteLine(json);

            return JsonConvert.DeserializeObject<T>(json, _jsonSerializerSettings);
        }

        private class Test<T>
        {
            public IFinalList<T> TheList { get; set; } 
        }
    }
}
