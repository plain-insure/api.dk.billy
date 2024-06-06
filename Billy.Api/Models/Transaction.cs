using System.Text.Json.Serialization;

namespace Billy.Api.Models
{
    public class Transaction
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public int TransactionNo { get; set; }
        public string VoucherNo { get; set; }
        public DateTime CreatedTime { get; set; }

        [JsonConverter(typeof(Converters.BillyDateConverter))]
        public DateTime EntryDate { get; set; }
        public string OriginatorReference { get; set; }
        public string OriginatorName { get; set; }
        public bool IsVoided { get; set; }
        public bool IsVoid { get; set; }
    }
}
