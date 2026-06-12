using System.Text.Json.Serialization;

namespace Billy.Api.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter<State>))]
    public enum State
    {
        Draft,
        Approved,
        Voided
    }
}
