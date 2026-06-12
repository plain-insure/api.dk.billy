using System.Text.Json.Serialization;

namespace Billy.Api.Models
{
    /// <summary>
    /// Represents a single ledger posting (Debit or Credit entry) generated as part of a <see cref="Transaction"/>.
    /// Postings are the lowest-level records in the double-entry bookkeeping system and are read-only.
    /// </summary>
    public class Posting : IEntity
    {
        /// <summary>Unique identifier assigned by the Billy API.</summary>
        public string Id { get; set; }

        /// <summary>ID of the organization this posting belongs to.</summary>
        public string OrganizationId { get; set; }

        /// <summary>ID of the <see cref="Transaction"/> that generated this posting.</summary>
        public string TransactionId { get; set; }

        /// <summary>Accounting entry date (date-only, serialized as <c>yyyy-MM-dd</c>).</summary>
        public DateOnly EntryDate { get; set; }

        /// <summary>Descriptive text shown in account statements for this posting.</summary>
        public string Text { get; set; }

        /// <summary>ID of the ledger <see cref="Account"/> this posting is applied to.</summary>
        public string AccountId { get; set; }

        /// <summary>Monetary amount of this posting in the transaction's currency.</summary>
        public decimal Amount { get; set; }

        /// <summary>Whether this is a <c>"Debit"</c> or <c>"Credit"</c> posting.</summary>
        public CashSide? Side { get; set; }

        /// <summary>ISO 4217 currency code of this posting.</summary>
        public string CurrencyId { get; set; }

        /// <summary>ID of the VAT/sales tax return period this posting contributes to, if applicable.</summary>
        public string SalesTaxReturnId { get; set; }

        /// <summary><c>true</c> if this posting has been voided.</summary>
        public bool IsVoided { get; set; }

        /// <summary><c>true</c> if this posting has been matched against a bank statement entry.</summary>
        public bool IsBankMatched { get; set; }

        /// <summary>Display order of this posting within the transaction (lower = first).</summary>
        public int Priority { get; set; }
    }
}
