namespace Billy.Api.Models
{
    public class DaybookRoot : Root
    {
        public Daybook? Daybook { get; set; }
        public List<Daybook>? Daybooks { get; set; }
    }

    public class Daybook : IEntity
    {
        public string? Id { get; set; }
        public string OrganizationId { get; set; }
        public string Name { get; set; }
        public bool IsTransactionSummaryEnabled { get; set; }
        public string? DefaultContraAccountId { get; set; }
        public List<DaybookBalanceAccount>? BalanceAccounts { get; set; }
    }

    public class DaybookBalanceAccount
    {
        public string? Id { get; set; }
        public string? DaybookId { get; set; }
        public string AccountId { get; set; }
        public int Priority { get; set; }
    }
}
