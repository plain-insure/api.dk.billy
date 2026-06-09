using Billy.Api.Utils;
using System.Text.Json.Serialization;

namespace Billy.Api.Models
{
    /// <summary>
    /// API response envelope for daybook transaction endpoints
    /// (<c>GET /v2/daybookTransactions</c>, <c>POST /v2/daybookTransactions</c>, etc.).
    /// </summary>
    public class DaybookTransactionRoot : Root
    {
        /// <summary>Single transaction returned by a Get request.</summary>
        public DaybookTransaction? DaybookTransaction { get; set; }

        /// <summary>List of transactions returned by a List or Create request.</summary>
        public List<DaybookTransaction>? DaybookTransactions { get; set; }

        /// <summary>
        /// Sideloaded transaction lines. Always populated by <see cref="DaybookTransactions"/>
        /// because lines are sideloaded by default in that repository.
        /// </summary>
        public List<DaybookTransactionLine>? DaybookTransactionLines { get; set; }
    }

    /// <summary>
    /// Represents a journal entry (transaction) within a <see cref="Daybook"/> in Billy.
    /// A transaction groups one or more <see cref="DaybookTransactionLine"/> records that must balance.
    /// </summary>
    public class DaybookTransaction : IEntity
    {
        /// <summary>Unique identifier assigned by the Billy API.</summary>
        public string? Id { get; set; }

        /// <summary>ID of the organization this transaction belongs to.</summary>
        public string OrganizationId { get; set; }

        /// <summary>ID of the <see cref="Daybook"/> this transaction is posted to.</summary>
        public string? DaybookId { get; set; }

        /// <summary>Timestamp when the transaction was created. Server-assigned; read-only.</summary>
        [JsonIgnoreOnWrite]
        public DateTime CreatedTime { get; set; }

        /// <summary>Accounting entry date (date-only, serialized as <c>yyyy-MM-dd</c>). Required.</summary>
        [JsonConverter(typeof(Converters.BillyDateConverter))]
        public DateTime EntryDate { get; set; }

        /// <summary>Voucher number used for filing and cross-referencing with source documents.</summary>
        public string? VoucherNo { get; set; }

        /// <summary>Short description of the transaction, shown in ledger views.</summary>
        public string? Description { get; set; }

        /// <summary>Extended narrative description providing additional context for the transaction.</summary>
        public string? ExtendedDescription { get; set; }

        /// <summary>
        /// API origin type identifying which system or integration created this transaction.
        /// </summary>
        public string? ApiType { get; set; }

        /// <summary>
        /// Workflow state: <c>"draft"</c>, <c>"approved"</c>, or <c>"voided"</c>.
        /// Only approved transactions affect the ledger.
        /// </summary>
        public string? State { get; set; }

        /// <summary>Internal transaction type used by Billy to categorize automatically created entries.</summary>
        public string? Type { get; set; }

        /// <summary>Display order of this transaction within the daybook (lower = first).</summary>
        public int? Priority { get; set; }

        /// <summary>Server-assigned IDs of the transaction's line items. Read-only.</summary>
        [JsonIgnoreOnWrite]
        public List<string>? LineIds { get; set; }

        /// <summary>
        /// Debit and credit lines that make up this transaction.
        /// Always populated by the <see cref="DaybookTransactions"/> repository.
        /// </summary>
        public List<DaybookTransactionLine>? Lines { get; set; }
    }

    /// <summary>
    /// A single debit or credit line within a <see cref="DaybookTransaction"/>.
    /// Each line posts an amount to a specific ledger account.
    /// </summary>
    public class DaybookTransactionLine : IEntity
    {
        /// <summary>Unique identifier of the line item.</summary>
        public string? Id { get; set; }

        /// <summary>ID of the <see cref="DaybookTransaction"/> this line belongs to.</summary>
        public string? DaybookTransactionId { get; set; }

        /// <summary>Descriptive text for this line, shown in the account statement.</summary>
        public string? Text { get; set; }

        /// <summary>ID of the ledger <see cref="Account"/> this line posts to.</summary>
        public string AccountId { get; set; }

        /// <summary>ID of the <see cref="TaxRate"/> applied to this line, or <c>null</c> if tax-exempt.</summary>
        public string? TaxRateId { get; set; }

        /// <summary>ID of the contra <see cref="TaxRate"/> for deductible tax splitting, if applicable.</summary>
        public string? ContraTaxRateId { get; set; }

        /// <summary>ID of the <see cref="Invoice"/> this line settles, if it is a payment line.</summary>
        public string? PaidInvoiceId { get; set; }

        /// <summary>External invoice ID settled by this line, used for cross-system reconciliation.</summary>
        public string? PaidExternalInvoiceId { get; set; }

        /// <summary>ID of the <see cref="Bill"/> this line settles, if it is a payment line.</summary>
        public string? PaidBillId { get; set; }

        /// <summary>
        /// ID of the offsetting <see cref="Account"/> for this line.
        /// When set, Billy automatically creates the balancing posting to this account.
        /// </summary>
        public string? ContraAccountId { get; set; }

        /// <summary>Amount for this line in the transaction's currency.</summary>
        public double? Amount { get; set; }

        /// <summary>Amount converted to the organization's base currency.</summary>
        public double? BaseAmount { get; set; }

        /// <summary>Whether this line is a <c>"debit"</c> or <c>"credit"</c> posting.</summary>
        public string Side { get; set; }

        /// <summary>ISO 4217 currency code for this line, if different from the base currency.</summary>
        public string? CurrencyId { get; set; }

        /// <summary>Display order of this line within the transaction (lower = first).</summary>
        public int? Priority { get; set; }
    }
}
