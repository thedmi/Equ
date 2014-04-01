namespace Belt.Serialization.JsonNet
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Belt.Lazy;

    using Newtonsoft.Json;

    public class LazyConverter : JsonConverter
    {
        private static readonly Type _markerType = typeof(ILazy);

        private static readonly MethodInfo _creatorMethod = typeof(Lazy).GetMethod("CreateEager");

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var innerValue = value.GetType().GetProperty("Value").GetValue(value);
            serializer.Serialize(writer, innerValue);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var nestedType = objectType.GenericTypeArguments.Single();
            
            var nestedValue = serializer.Deserialize(reader, nestedType);
            return _creatorMethod.MakeGenericMethod(nestedType).Invoke(null, new[] { nestedValue });
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == _markerType || objectType.GetInterfaces().Any(i => i == _markerType);
        }
    }
}
