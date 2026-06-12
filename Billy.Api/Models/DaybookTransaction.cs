using Billy.Api.Utils;

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
        public DateOnly EntryDate { get; set; }

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
        public State? State { get; set; }

        /// <summary>Internal transaction type used by Billy to categorize automatically created entries.</summary>
        public string? Type { get; set; }

        /// <summary>Display order of this transaction within the daybook (lower = first).</summary>
        public int? Priority { get; set; }

        /// <summary>Server-assigned IDs of the transaction's line items. Read-only.</summary>
        [JsonIgnoreOnWrite]
        public List<string>? LineIds { get; set; }

        /// <summary>
        /// Debit and Credit lines that make up this transaction.
        /// Always populated by the <see cref="DaybookTransactions"/> repository.
        /// </summary>
        public List<DaybookTransactionLine>? Lines { get; set; }
    }
}
