namespace Billy.Api.Models
{
    /// <summary>
    /// A single Debit or Credit line within a <see cref="DaybookTransaction"/>.
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
        public decimal? Amount { get; set; }

        /// <summary>Amount converted to the organization's base currency.</summary>
        public decimal? BaseAmount { get; set; }

        /// <summary>Whether this line is a <c>"Debit"</c> or <c>"Credit"</c> posting.</summary>
        public CashSide Side { get; set; }

        /// <summary>ISO 4217 currency code for this line, if different from the base currency.</summary>
        public string? CurrencyId { get; set; }

        /// <summary>Display order of this line within the transaction (lower = first).</summary>
        public int? Priority { get; set; }
    }
}
