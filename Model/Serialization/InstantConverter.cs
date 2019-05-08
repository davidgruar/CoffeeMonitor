namespace CoffeeMonitor.Model.Serialization
{
    using System;

    using Newtonsoft.Json;

    using NodaTime;
    using NodaTime.Serialization.JsonNet;

    public class InstantConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            NodaConverters.InstantConverter.WriteJson(writer, value, serializer);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Handle case where we couldn't configure the Json serializer settings (e.g. in durable functions)
            if (reader.Value is DateTime dt)
            {
                return Instant.FromDateTimeUtc(dt);
            }

            return NodaConverters.InstantConverter.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override bool CanConvert(Type objectType) =>
            NodaConverters.InstantConverter.CanConvert(objectType);
    }
}