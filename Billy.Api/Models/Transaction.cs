using System.Text.Json.Serialization;

namespace Billy.Api.Models
{
    /// <summary>
    /// Indicates which side of a double-entry bookkeeping entry a <see cref="Transaction"/> affects.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<TransactionSide>))]
    public enum TransactionSide
    {
        /// <summary>Debit side — increases assets and expenses; decreases liabilities, equity, and income.</summary>
        Debit,

        /// <summary>Credit side — increases liabilities, equity, and income; decreases assets and expenses.</summary>
        Credit
    }

    /// <summary>
    /// Represents an accounting transaction generated when an invoice, bill, or daybook entry is approved.
    /// Transactions are sideloaded in some API responses and are read-only.
    /// </summary>
    public class Transaction : IEntity
    {
        /// <summary>Unique identifier assigned by the Billy API.</summary>
        public string Id { get; set; }

        /// <summary>ID of the organization this transaction belongs to.</summary>
        public string OrganizationId { get; set; }

        /// <summary>Sequential transaction number assigned by Billy for ordering and reference.</summary>
        public int TransactionNo { get; set; }

        /// <summary>Voucher number linking this transaction to a source document.</summary>
        public string VoucherNo { get; set; }

        /// <summary>Timestamp when this transaction was created.</summary>
        public DateTime createdTime { get; set; }

        /// <summary>Accounting entry date (date-only, serialized as <c>yyyy-MM-dd</c>).</summary>
        [JsonConverter(typeof(Converters.BillyDateConverter))]
        public DateTime EntryDate { get; set; }

        /// <summary>Reference string identifying the source document (e.g. <c>"invoice:inv-123"</c>).</summary>
        public string OriginatorReference { get; set; }

        /// <summary>Human-readable name of the originating document type (e.g. <c>"Invoice"</c>).</summary>
        public string OriginatorName { get; set; }

        /// <summary><c>true</c> if this transaction has been voided and its postings reversed.</summary>
        public bool IsVoided { get; set; }

        /// <summary>Alias for <see cref="IsVoided"/>; present for API compatibility.</summary>
        public bool IsVoid { get; set; }

        /// <summary>Identifier of the user or system that created this transaction.</summary>
        public string Creator { get; set; }

        /// <summary>Total amount of this transaction in the originator's currency.</summary>
        public int Amount { get; set; }

        /// <summary>ISO 4217 currency code of the originating document.</summary>
        public string OriginatorCurrencyId { get; set; }

        /// <summary>Resolved originator currency object, when sideloaded by the API.</summary>
        public Currency OriginatorCurrency { get; set; }

        /// <summary>Type of the originating document (e.g. <c>"invoice"</c>, <c>"bill"</c>).</summary>
        public string OriginatorType { get; set; }

        /// <summary>Whether the transaction increases (Debit) or decreases (Credit) the originator's account.</summary>
        public TransactionSide Side { get; set; }
    }
}
