namespace Belt.Serialization.JsonNet
{
    using Newtonsoft.Json;

    public static class JsonNetExtensions
    {
        public static JsonSerializerSettings ConfigureForBelt(this JsonSerializerSettings settings)
        {
            settings.Converters.Add(new FinalListConverter());
            settings.Converters.Add(new MaybeConverter());
            settings.Converters.Add(new LazyConverter());

            return settings;
        }

        public static JsonSerializer ConfigureForBelt(this JsonSerializer serializer)
        {
            serializer.Converters.Add(new FinalListConverter());
            serializer.Converters.Add(new MaybeConverter());
            serializer.Converters.Add(new LazyConverter());

            return serializer;
        }
    }
}
