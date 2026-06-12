using Billy.Api.Utils;
using System.Text.Json.Serialization;

namespace Billy.Api.Models
{
    /// <summary>
    /// API response envelope for invoice endpoints (<c>GET /v2/invoices</c>, <c>POST /v2/invoices</c>, etc.).
    /// </summary>
    public class InvoiceRoot : Root
    {
        /// <summary>Single invoice returned by a Get request.</summary>
        public Invoice Invoice { get; set; }

        /// <summary>List of invoices returned by a List or Create request.</summary>
        public List<Invoice> Invoices { get; set; }

        /// <summary>Sideloaded line items for the returned invoices.</summary>
        public List<InvoiceLine> InvoiceLines { get; set; }

        /// <summary>Sideloaded organization records associated with the invoices.</summary>
        public List<Organization> Organizations { get; set; }

        /// <summary>Accounting transactions generated when an invoice is approved.</summary>
        public List<Transaction> Transactions { get; set; }

        /// <summary>Ledger postings associated with the invoice's transactions.</summary>
        public List<Posting> Postings { get; set; }
    }

    /// <summary>
    /// A payment method option that can be presented to the customer on an invoice.
    /// </summary>
    public class InvoicePaymentMethod
    {
        /// <summary>ID of the payment method (e.g. a MobilePay or card gateway integration).</summary>
        public string? PaymentMethodId { get; set; }
    }

    /// <summary>
    /// Represents a sales invoice in Billy. Invoices track amounts owed by customers and can be
    /// approved, credited, or paid via <see cref="BankPayments"/>.
    /// </summary>
    public class Invoice : IEntity
    {
        /// <summary>Unique identifier assigned by the Billy API.</summary>
        public string Id { get; set; }

        /// <summary>ID of the organization that issued this invoice.</summary>
        public string OrganizationId { get; set; }

        /// <summary>
        /// Invoice type: <c>"invoice"</c> for a normal sales invoice or <c>"creditNote"</c> for a Credit note.
        /// </summary>
        public string Type { get; set; }

        /// <summary>Timestamp when the invoice was created. Server-assigned; read-only.</summary>
        [JsonIgnoreOnWrite]
        public DateTime CreatedTime { get; set; }

        /// <summary>Timestamp when the invoice was approved. <c>null</c> if still in draft.</summary>
        public DateTime? ApprovedTime { get; set; }

        /// <summary>ID of the customer <see cref="Contact"/> this invoice is addressed to.</summary>
        public string ContactId { get; set; }

        /// <summary>ID of the <see cref="ContactPerson"/> to whom the invoice is specifically addressed.</summary>
        public string? AttContactPersonId { get; set; }

        /// <summary>Invoice issue date (date-only, serialized as <c>yyyy-MM-dd</c>).</summary>
        public DateOnly EntryDate { get; set; }

        /// <summary>
        /// Payment terms mode: <c>"net"</c>, <c>"dueDate"</c>, <c>"paid"</c>, etc.
        /// Determines how <see cref="DueDate"/> is calculated.
        /// </summary>
        public string? PaymentTermsMode { get; set; }

        /// <summary>Number of days used together with <see cref="PaymentTermsMode"/> to calculate the due date.</summary>
        public int? PaymentTermsDays { get; set; }

        /// <summary>Payment due date (date-only, serialized as <c>yyyy-MM-dd</c>). Skipped when default.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateOnly? DueDate { get; set; }

        /// <summary>
        /// Workflow state: <c>"draft"</c>, <c>"approved"</c>, or <c>"voided"</c>.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Email delivery state: <c>"unsent"</c>, <c>"sent"</c>, or <c>"opened"</c>.
        /// </summary>
        public string SentState { get; set; }

        /// <summary>External identifier for cross-referencing with other systems.</summary>
        public string? ExternalId { get; set; }

        /// <summary>ID of the quote this invoice was created from, if any.</summary>
        public string? QuoteId { get; set; }

        /// <summary>
        /// The human-readable invoice number (e.g. <c>"2024-0042"</c>).
        /// Assigned by the API on approval based on the organization's invoice number sequence.
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// Whether line amounts include tax: <c>"excl"</c> for tax-exclusive, <c>"incl"</c> for tax-inclusive.
        /// </summary>
        public TaxMode TaxMode { get; set; }

        /// <summary>Total net amount (excluding tax) in the invoice's currency. Computed from lines; read-only.</summary>
        [JsonIgnoreOnWrite]
        public decimal Amount { get; set; }

        /// <summary>Total tax amount in the invoice's currency. Computed from lines; read-only.</summary>
        [JsonIgnoreOnWrite]
        public decimal Tax { get; set; }

        /// <summary>Total gross amount (including tax) in the invoice's currency. Computed from lines; read-only.</summary>
        [JsonIgnoreOnWrite]
        public decimal GrossAmount { get; set; }

        /// <summary>ISO 4217 currency code for this invoice (e.g. <c>"DKK"</c>, <c>"EUR"</c>).</summary>
        public string CurrencyId { get; set; }

        /// <summary>Exchange rate relative to the organization's base currency at the time of the invoice.</summary>
        public decimal ExchangeRate { get; set; }

        /// <summary>Outstanding amount remaining to be paid, in the invoice's currency. Read-only.</summary>
        [JsonIgnoreOnWrite]
        public decimal Balance { get; set; }

        /// <summary><c>true</c> when the full invoice amount has been settled. Read-only.</summary>
        [JsonIgnoreOnWrite]
        public bool IsPaid { get; set; }

        /// <summary>ID of the invoice that this Credit note credits, when this invoice is a Credit note.</summary>
        public string? CreditedInvoiceId { get; set; }

        /// <summary>Message displayed to the customer on the invoice (e.g. a personal note or reference).</summary>
        public string ContactMessage { get; set; }

        /// <summary>Default description text pre-filled on new line items.</summary>
        public string LineDescription { get; set; }

        /// <summary>
        /// ID of the bank account whose payment details are printed on the invoice.
        /// Server-assigned based on the organization's default; read-only.
        /// </summary>
        [JsonIgnoreOnWrite]
        public string BankAccountId { get; set; }

        /// <summary>ID of the recurring invoice template that generated this invoice, if any.</summary>
        public string? RecurringInvoiceId { get; set; }

        /// <summary>ID of the document template used when rendering this invoice as a PDF.</summary>
        public string? TemplateId { get; set; }

        /// <summary>URL to download the invoice PDF. Server-assigned; read-only.</summary>
        [JsonIgnoreOnWrite]
        public string DownloadUrl { get; set; }

        /// <summary>Customer's own purchase order number for cross-referencing.</summary>
        public string? OrderNo { get; set; }

        /// <summary>Payment method options presented to the customer on the invoice.</summary>
        public List<InvoicePaymentMethod>? PaymentMethods { get; set; }

        /// <summary>
        /// Line items on this invoice. Required when creating an invoice;
        /// embed them in the POST body to create lines atomically.
        /// </summary>
        public List<InvoiceLine> Lines { get; set; }
    }
}
