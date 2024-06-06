using System.Text.Json.Serialization;

namespace Billy.Api.Models
{
    public class Invoice
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string Type { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ApprovedTime { get; set; }
        public string ContactId { get; set; }
        public object AttContactPersonId { get; set; }

        [JsonConverter(typeof (Converters.BillyDateConverter))]
        public DateTime EntryDate { get; set; }
        public object PaymentTermsMode { get; set; }
        public object PaymentTermsDays { get; set; }

        [JsonConverter(typeof(Converters.BillyDateConverter))]
        public DateTime DueDate { get; set; }
        public string State { get; set; }
        public string SentState { get; set; }
        public string InvoiceNo { get; set; }
        public string TaxMode { get; set; }
        public double Amount { get; set; }
        public double Tax { get; set; }
        public string CurrencyId { get; set; }
        public int ExchangeRate { get; set; }
        public int Balance { get; set; }
        public bool IsPaid { get; set; }
        public object CreditedInvoiceId { get; set; }
        public string ContactMessage { get; set; }
        public string LineDescription { get; set; }
        public string BankAccountId { get; set; }
        public object RecurringInvoiceId { get; set; }
        public string DownloadUrl { get; set; }
        public List<InvoiceLine> Lines { get; set; }
    }

    public class InvoiceRoot
    {
        public Meta Meta { get; set; }
        public Invoice Invoice { get; set; }
        public List<Invoice> Invoices { get; set; }
        public List<InvoiceLine> InvoiceLines { get; set; }
        public List<Organization> Organizations { get; set; }
        public List<Transaction> Transactions { get; set; }
        public List<Posting> Postings { get; set; }
    }
}
