using System.Text.Json.Serialization;

namespace Billy.Api.Models
{

    public enum TransactionSide
    {
        Debit,
        Credit
    }

    public class Transaction : IEntity
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public int TransactionNo { get; set; }
        public string VoucherNo { get; set; }
        public DateTime createdTime { get; set; }

        [JsonConverter(typeof(Converters.BillyDateConverter))]
        public DateTime EntryDate { get; set; }
        public string OriginatorReference { get; set; }
        public string OriginatorName { get; set; }
        public bool IsVoided { get; set; }
        public bool IsVoid { get; set; }
        public string Creator { get; set; }
        public int Amount { get; set; }
        public string OriginatorCurrencyId { get; set; }
        public Currency OriginatorCurrency { get; set; }

        public string OriginatorType { get; set; }
        public TransactionSide Side { get; set; }
    }
}
