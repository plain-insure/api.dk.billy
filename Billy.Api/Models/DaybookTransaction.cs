using Billy.Api.Utils;
using System.Text.Json.Serialization;

namespace Billy.Api.Models
{
    public class DaybookTransactionIdList
    {
        public List<string>? DaybookTransactions { get; set; }
    }

    public class DaybookTransactionRoot : Root<DaybookTransactionIdList>
    {
        public DaybookTransaction? DaybookTransaction { get; set; }
        public List<DaybookTransaction>? DaybookTransactions { get; set; }
        public List<DaybookTransactionLine>? DaybookTransactionLines { get; set; }
    }

    public class DaybookTransaction : IEntity
    {
        public string? Id { get; set; }
        public string OrganizationId { get; set; }
        public string? DaybookId { get; set; }

        [JsonIgnoreOnWrite]
        public DateTime CreatedTime { get; set; }

        [JsonConverter(typeof(Converters.BillyDateConverter))]
        public DateTime EntryDate { get; set; }

        public string? VoucherNo { get; set; }
        public string? Description { get; set; }
        public string? ExtendedDescription { get; set; }
        public string? ApiType { get; set; }
        public string? State { get; set; }
        public string? Type { get; set; }
        public int? Priority { get; set; }

        [JsonIgnoreOnWrite]
        public List<string>? LineIds { get; set; }

        public List<DaybookTransactionLine>? Lines { get; set; }
    }

    public class DaybookTransactionLine : IEntity
    {
        public string? Id { get; set; }
        public string? DaybookTransactionId { get; set; }
        public string? Text { get; set; }
        public string AccountId { get; set; }
        public string? TaxRateId { get; set; }
        public string? ContraTaxRateId { get; set; }
        public string? PaidInvoiceId { get; set; }
        public string? PaidExternalInvoiceId { get; set; }
        public string? PaidBillId { get; set; }
        public string? ContraAccountId { get; set; }
        public double? Amount { get; set; }
        public double? BaseAmount { get; set; }
        public string Side { get; set; }
        public string? CurrencyId { get; set; }
        public int? Priority { get; set; }
    }
}
