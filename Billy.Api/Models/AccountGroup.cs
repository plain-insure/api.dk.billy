namespace Billy.Api.Models
{
    /// <summary>
    /// API response envelope for account group endpoints (<c>GET /v2/accountGroups</c>, etc.).
    /// </summary>
    public class AccountGroupRoot : Root
    {
        /// <summary>Single account group returned by a Get request.</summary>
        public AccountGroup AccountGroup { get; set; }

        /// <summary>List of account groups returned by a List request.</summary>
        public List<AccountGroup> AccountGroups { get; set; }
    }

    /// <summary>
    /// Represents a grouping of ledger accounts in Billy's chart of accounts.
    /// Account groups organize accounts into sections such as assets, liabilities, income, and expenses.
    /// Account groups are read-only through this library.
    /// </summary>
    public class AccountGroup : IEntity
    {
        /// <summary>Unique identifier assigned by the Billy API.</summary>
        public string Id { get; set; }

        /// <summary>ID of the organization this account group belongs to.</summary>
        public string OrganizationId { get; set; }

        /// <summary>Account number prefix or range start for this group.</summary>
        public int AccountNo { get; set; }

        /// <summary>Display name of the account group (e.g. <c>"Current Assets"</c>).</summary>
        public string Name { get; set; }

        /// <summary>
        /// Group type controlling how this group is displayed and summed in financial reports
        /// (e.g. <c>"heading"</c>, <c>"sum"</c>, <c>"normal"</c>).
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// ID of the account nature (asset, liability, equity, income, or expense) for this group.
        /// Immutable after creation.
        /// </summary>
        public string NatureId { get; set; }

        /// <summary>
        /// Reference to another group whose total is added into this group's sum,
        /// used for summary groups in financial reports.
        /// </summary>
        public string? SumFrom { get; set; }

        /// <summary>Visual style hint used by the Billy UI when rendering this group in reports.</summary>
        public string? Style { get; set; }

        /// <summary>Sort order of this group relative to other groups (lower = first).</summary>
        public int Priority { get; set; }

        /// <summary>Start of the account number range included in this group, if range-based.</summary>
        public int? IntervalFrom { get; set; }

        /// <summary>End of the account number range included in this group, if range-based.</summary>
        public int? IntervalTo { get; set; }

        /// <summary>
        /// When <c>true</c>, accounts in this group can be used as payment accounts
        /// for recording payment amounts.
        /// </summary>
        public bool? AllowPaymentAmounts { get; set; }

        /// <summary>ID of the predefined account group template this group was created from, if any.</summary>
        public string? PredefinedAccountGroupId { get; set; }
    }
}
