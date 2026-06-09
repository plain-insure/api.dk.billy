using System.Text.Json.Serialization;

namespace Billy.Api.Models
{
    /// <summary>
    /// Workflow states for a <see cref="Bill"/>. Serialized as camelCase strings in the Billy API.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<BillStates>))]
    public enum BillStates
    {
        /// <summary>The bill has been created but not yet approved. It does not affect the ledger.</summary>
        draft,

        /// <summary>The bill has been approved and posted to the ledger.</summary>
        approved,

        /// <summary>The bill has been voided and its ledger postings reversed.</summary>
        voided
    }
}
