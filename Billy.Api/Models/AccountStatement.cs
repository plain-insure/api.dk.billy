namespace Billy.Api.Models
{
    /// <summary>
    /// API response envelope for the account statement endpoint (<c>GET /v2/accounts/:id/statement</c>).
    /// </summary>
    public class AccountStatementRoot : Root
    {
        /// <summary>The account statement data.</summary>
        public AccountStatement AccountStatement { get; set; }
    }

    /// <summary>
    /// Represents a period-based account statement, listing all postings to a specific ledger account
    /// along with running balances. Returned read-only by the Billy API.
    /// </summary>
    public class AccountStatement
    {
        /// <summary>Opening balance of the account at the start of the requested period.</summary>
        public int StartBalance { get; set; }

        /// <summary>
        /// <c>true</c> if the statement was cut short because it exceeded the maximum row limit.
        /// Narrow the date range to retrieve the full data.
        /// </summary>
        public bool IsTruncated { get; set; }

        /// <summary>Individual posting rows that make up the statement, in chronological order.</summary>
        public Row[] Rows { get; set; }

        /// <summary>A single row in an <see cref="AccountStatement"/>, corresponding to one ledger posting.</summary>
        public class Row
        {
            /// <summary>Unique identifier of this posting row.</summary>
            public string Id { get; set; }

            /// <summary>ID of the <see cref="Transaction"/> this row belongs to.</summary>
            public string TransactionId { get; set; }

            /// <summary>Sequential transaction number for display and ordering.</summary>
            public int TransactionNo { get; set; }

            /// <summary>Voucher number of the transaction, linking it to a source document.</summary>
            public string VoucherNo { get; set; }

            /// <summary>Human-readable name of the document type that originated this transaction.</summary>
            public string OriginatorName { get; set; }

            /// <summary>Accounting entry date of the posting (formatted as a string by the API).</summary>
            public string EntryDate { get; set; }

            /// <summary>Descriptive text shown for this posting in the statement.</summary>
            public string Text { get; set; }

            /// <summary>Net amount of the posting in the account's currency.</summary>
            public string BaseAmount { get; set; }

            /// <summary>ISO 4217 currency code of this posting.</summary>
            public string CurrencyId { get; set; }

            /// <summary>Resolved currency object for this posting.</summary>
            public Currency Currency { get; set; }

            /// <summary>Tax amount included in this posting.</summary>
            public int Tax { get; set; }

            /// <summary><c>true</c> if this posting has been voided.</summary>
            public bool IsVoided { get; set; }

            /// <summary>Gross amount (including tax) of the posting.</summary>
            public int GrossAmount { get; set; }

            /// <summary>Credit component of this posting.</summary>
            public int Credit { get; set; }

            /// <summary>Running balance of the account after this posting.</summary>
            public int Balance { get; set; }

            /// <summary><c>true</c> if this row has special display treatment in the Billy UI.</summary>
            public bool IsSpecial { get; set; }
        }
    }
}
