using Billy.Api.Utils;
using System.Text.Json.Serialization;

namespace Billy.Api.Models
{

    public class InvoiceIdList
    {
        public List<string>? Invoices { get; set; }

    }

    public class InvoiceRoot : Root<InvoiceIdList>
    {
        public Invoice Invoice { get; set; }
        public List<Invoice> Invoices { get; set; }
        public List<InvoiceLine> InvoiceLines { get; set; }
        public List<Organization> Organizations { get; set; }
        public List<Transaction> Transactions { get; set; }
        public List<Posting> Postings { get; set; }
    }

    public class InvoicePaymentMethod
    {
        public string? PaymentMethodId { get; set; }
    }

    public class Invoice : IEntity
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string Type { get; set; }

        [JsonIgnoreOnWrite]
        public DateTime CreatedTime { get; set; }
        public DateTime? ApprovedTime { get; set; }
        public string ContactId { get; set; }
        public string? AttContactPersonId { get; set; }

        [JsonConverter(typeof(Converters.BillyDateConverter))]
        public DateTime EntryDate { get; set; }
        public string? PaymentTermsMode { get; set; }
        public int? PaymentTermsDays { get; set; }

        [JsonConverter(typeof(Converters.BillyDateConverter))]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateTime DueDate { get; set; }
        public string State { get; set; }
        public string SentState { get; set; }
        public string? ExternalId { get; set; }
        public string? QuoteId { get; set; }
        public string InvoiceNo { get; set; }
        public string TaxMode { get; set; }
        [JsonIgnoreOnWrite]
        public double Amount { get; set; }
        [JsonIgnoreOnWrite]
        public double Tax { get; set; }
        [JsonIgnoreOnWrite]
        public double GrossAmount { get; set; }
        public string CurrencyId { get; set; }
        public int ExchangeRate { get; set; }
        [JsonIgnoreOnWrite]
        public int Balance { get; set; }
        [JsonIgnoreOnWrite]
        public bool IsPaid { get; set; }
        public string? CreditedInvoiceId { get; set; }
        public string ContactMessage { get; set; }
        public string LineDescription { get; set; }

        [JsonIgnoreOnWrite]
        public string BankAccountId { get; set; }
        public string? RecurringInvoiceId { get; set; }
        public string? TemplateId { get; set; }

        [JsonIgnoreOnWrite]
        public string DownloadUrl { get; set; }
        public string? OrderNo { get; set; }
        public List<InvoicePaymentMethod>? PaymentMethods { get; set; }
        public List<InvoiceLine> Lines { get; set; }
    }

}
