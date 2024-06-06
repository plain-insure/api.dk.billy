using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BillyService.Converters
{
    public class BillyDateConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var token = reader.GetString();
            if (token == null)
                return DateTime.MinValue;

            return DateTime.ParseExact(token, "yyyy-MM-dd", null);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
        }
    }
}
