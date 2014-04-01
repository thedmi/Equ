namespace Belt.Serialization.JsonNet
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection;

    using Belt.FinalList;

    using Newtonsoft.Json;

    public class FinalListConverter : JsonConverter
    {
        private static readonly Type _finalListMarkerType = typeof(IFinalList);

        private static readonly Type _immutableListType = typeof(ImmutableList<>);

        private static readonly MethodInfo _genericFinalListCreatorMethod = typeof(FinalListExtensions).GetMethod("ToFinalList");

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var enumerable = value as IEnumerable;
            if (enumerable == null)
            {
                throw new ArgumentException("Value is not an IEnumerable, cannot serialize", "value");
            }

            writer.WriteStartArray();

            foreach (var val in enumerable)
            {
                serializer.Serialize(writer, val);
            }

            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var nativeType = _immutableListType.MakeGenericType(objectType.GenericTypeArguments);
            var deserialize = serializer.Deserialize(reader, nativeType);

            var creatorMethod = _genericFinalListCreatorMethod.MakeGenericMethod(objectType.GenericTypeArguments);
            var finalList = creatorMethod.Invoke(null, new[] { deserialize });

            return finalList;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == _finalListMarkerType || objectType.GetInterfaces().Any(i => i == _finalListMarkerType);
        }
    }
}
