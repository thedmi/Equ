namespace Belt.Serialization.JsonNet
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Belt.Maybe;

    using Newtonsoft.Json;

    public class MaybeConverter : JsonConverter
    {
        private static readonly Type _markerType = typeof(IMaybe);

        private static readonly MethodInfo _emptyCreatorMethod = typeof(Maybe).GetMethod("Empty");
        private static readonly MethodInfo _existingCreatorMethod = typeof(Maybe).GetMethod("Is");

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var maybe = (IMaybe)value;

            if (maybe.Exists)
            {
                var innerValue = maybe.GetType().GetProperty("It").GetValue(maybe);
                serializer.Serialize(writer, innerValue);
            }
            else
            {
                serializer.Serialize(writer, null);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var nestedType = objectType.GenericTypeArguments.Single();

            if (reader.TokenType == JsonToken.Null)
            {
                return _emptyCreatorMethod.MakeGenericMethod(nestedType).Invoke(null, new object[] { });
            }

            var nestedValue = serializer.Deserialize(reader, nestedType);
            return _existingCreatorMethod.MakeGenericMethod(nestedType).Invoke(null, new[] { nestedValue });
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == _markerType || objectType.GetInterfaces().Any(i => i == _markerType);
        }
    }
}
