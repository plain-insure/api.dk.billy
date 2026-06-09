namespace Billy.Api.Models
{
    /// <summary>
    /// API response envelope for daybook endpoints (<c>GET /v2/daybooks</c>, <c>POST /v2/daybooks</c>, etc.).
    /// </summary>
    public class DaybookRoot : Root
    {
        /// <summary>Single daybook returned by a Get request.</summary>
        public Daybook? Daybook { get; set; }

        /// <summary>List of daybooks returned by a List or Create request.</summary>
        public List<Daybook>? Daybooks { get; set; }
    }

    /// <summary>
    /// Represents a daybook (accounting journal) in Billy. Daybooks are used to group and organize
    /// <see cref="DaybookTransaction"/> entries for manual bookkeeping.
    /// </summary>
    public class Daybook : IEntity
    {
        /// <summary>Unique identifier assigned by the Billy API.</summary>
        public string? Id { get; set; }

        /// <summary>ID of the organization this daybook belongs to.</summary>
        public string OrganizationId { get; set; }

        /// <summary>Display name of the daybook (e.g. <c>"General Journal"</c>, <c>"Bank"</c>).</summary>
        public string Name { get; set; }

        /// <summary>
        /// When <c>true</c>, a summary transaction is posted to the balance accounts instead of
        /// individual line postings, keeping the main ledger cleaner.
        /// </summary>
        public bool IsTransactionSummaryEnabled { get; set; }

        /// <summary>
        /// ID of the default contra <see cref="Account"/> used when entering transactions in this daybook.
        /// Acts as the offsetting account for the debit or credit side.
        /// </summary>
        public string? DefaultContraAccountId { get; set; }

        /// <summary>
        /// Accounts used as the settlement (cash/bank) side of transactions in this daybook.
        /// Transactions are posted to both the line account and one of these balance accounts.
        /// </summary>
        public List<DaybookBalanceAccount>? BalanceAccounts { get; set; }
    }

    /// <summary>
    /// Links a balance (cash or bank) <see cref="Account"/> to a <see cref="Daybook"/>.
    /// The balance account receives the offsetting posting for each transaction line.
    /// </summary>
    public class DaybookBalanceAccount
    {
        /// <summary>Unique identifier of this daybook–account link.</summary>
        public string? Id { get; set; }

        /// <summary>ID of the <see cref="Daybook"/> this entry belongs to.</summary>
        public string? DaybookId { get; set; }

        /// <summary>ID of the balance <see cref="Account"/> (typically a bank or cash account).</summary>
        public string AccountId { get; set; }

        /// <summary>Order in which this account appears in the daybook's account list (lower = first).</summary>
        public int Priority { get; set; }
    }
}
