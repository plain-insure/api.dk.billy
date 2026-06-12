using Billy.Api.Utils;
using System.Text.Json.Serialization;

namespace Billy.Api.Models
{
    /// <summary>
    /// API response envelope for bill endpoints (<c>GET /v2/bills</c>, <c>POST /v2/bills</c>, etc.).
    /// </summary>
    public class BillRoot : Root
    {
        /// <summary>Single bill returned by a Get request.</summary>
        public Bill? Bill { get; set; }

        /// <summary>List of bills returned by a List or Create request.</summary>
        public List<Bill>? Bills { get; set; }

        /// <summary>Sideloaded line items. Populated when <see cref="Bills.SideloadLines"/> is enabled.</summary>
        public List<BillLine>? BillLines { get; set; }

        /// <summary>Accounting transactions generated when a bill is approved.</summary>
        public List<Transaction>? Transactions { get; set; }

        /// <summary>Ledger postings associated with the bill's transactions.</summary>
        public List<Posting>? Postings { get; set; }
    }

    /// <summary>
    /// Represents a supplier invoice (purchase invoice / bill) in Billy.
    /// Bills track amounts owed to suppliers and can be approved, voided, or paid via <see cref="BankPayments"/>.
    /// </summary>
    public class Bill : IEntity
    {
        /// <summary>Unique identifier assigned by the Billy API.</summary>
        public string? Id { get; set; }

        /// <summary>ID of the organization this bill belongs to.</summary>
        public string OrganizationId { get; set; }

        /// <summary>
        /// Bill type: <c>"bill"</c> for a normal supplier invoice or <c>"creditNote"</c> for a Credit note.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>Timestamp when the bill was created. Server-assigned; read-only.</summary>
        [JsonIgnoreOnWrite]
        public DateTime CreatedTime { get; set; }

        /// <summary>Timestamp when the bill was approved. <c>null</c> if the bill is still in draft.</summary>
        public DateTime? ApprovedTime { get; set; }

        /// <summary>ID of the supplier <see cref="Contact"/> this bill is from.</summary>
        public string ContactId { get; set; }

        /// <summary>Snapshot of the supplier's name at the time the bill was created.</summary>
        public string? ContactName { get; set; }

        /// <summary>Accounting entry date (date-only, serialized as <c>yyyy-MM-dd</c>).</summary>
        public DateOnly EntryDate { get; set; }

        /// <summary>ID of the bank account used when recording payment, if the bill has been paid.</summary>
        public string? PaymentAccountId { get; set; }

        /// <summary>Date the bill was paid (date-only, serialized as <c>yyyy-MM-dd</c>).</summary>
        public DateOnly? PaymentDate { get; set; }

        /// <summary>Payment due date (date-only, serialized as <c>yyyy-MM-dd</c>).</summary>
        public DateOnly? DueDate { get; set; }

        /// <summary>
        /// When <c>true</c>, the bill is a "bare" bill with no line items — amount and description
        /// are taken from <see cref="Amount"/> and <see cref="LineDescription"/> directly.
        /// </summary>
        public bool IsBare { get; set; }

        /// <summary>Current workflow state of the bill.</summary>
        public State State { get; set; }

        /// <summary>The supplier's own invoice number, used for cross-referencing.</summary>
        public string? SuppliersInvoiceNo { get; set; }

        /// <summary>
        /// Whether line amounts include tax: <c>"excl"</c> for tax-exclusive, <c>"incl"</c> for tax-inclusive.
        /// </summary>
        public TaxMode TaxMode { get; set; }

        /// <summary>Internal voucher number assigned by Billy for accounting purposes.</summary>
        public string? VoucherNo { get; set; }

        /// <summary>Total net amount (excluding tax) in the bill's currency. Computed from lines; read-only.</summary>
        public decimal Amount { get; set; }

        /// <summary>Total tax amount in the bill's currency. Computed from lines; read-only.</summary>
        public decimal Tax { get; set; }

        /// <summary>ISO 4217 currency code for this bill (e.g. <c>"DKK"</c>, <c>"EUR"</c>).</summary>
        public string? CurrencyId { get; set; }

        /// <summary>
        /// Resolved currency object. Populated by the repository after a Get or List call.
        /// Not serialized — use <see cref="CurrencyId"/> when writing.
        /// </summary>
        [JsonIgnore]
        public Currency? Currency { get => _currency; set { _currency = value; CurrencyId = value?.Id; } }
        internal Currency? _currency;

        /// <summary>Exchange rate relative to the organization's base currency at the time of the bill.</summary>
        public decimal ExchangeRate { get; set; }

        /// <summary>Outstanding amount remaining to be paid, in the bill's currency.</summary>
        public decimal Balance { get; set; }

        /// <summary><c>true</c> when the full bill amount has been settled.</summary>
        public bool IsPaid { get; set; }

        /// <summary>Description text used on bare bills (no line items).</summary>
        public string? LineDescription { get; set; }

        /// <summary>ID of the original bill that this bill credits, when this is a Credit note.</summary>
        public string? CreditedBillId { get; set; }

        /// <summary>Source system identifier; used for integrations to track the origin of the bill.</summary>
        public string? Source { get; set; }

        /// <summary>Free-text subject line for the bill.</summary>
        public string? Subject { get; set; }

        /// <summary>
        /// Line items on this bill. Populated when sideloaded via <see cref="Bills.SideloadLines"/>,
        /// or when embedded at creation time.
        /// </summary>
        public List<BillLine>? Lines { get; set; }

        /// <summary>Server-assigned IDs of the line items. Read-only.</summary>
        [JsonIgnoreOnWrite]
        public List<string>? LineIds { get; set; }
    }
}
