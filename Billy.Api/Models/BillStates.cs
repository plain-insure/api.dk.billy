using System.Text.Json.Serialization;

namespace Billy.Api.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter<BillStates>))]
    public enum BillStates
    {
        draft,
        approved,
        voided
    }
}
