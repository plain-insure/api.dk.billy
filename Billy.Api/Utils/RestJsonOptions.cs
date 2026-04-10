using System.Text.Json;
using System.Text.Json.Serialization;

namespace Billy.Api.Utils
{
    public static class RestJsonOptions
    {
        public static readonly JsonSerializerOptions Instance = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            TypeInfoResolver = new IgnoreOnWriteTypeInfoResolver(),
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            Converters = {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
            // add your other shared settings here:
            // DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            // Converters = { ... }
        };
    }
}
