namespace Billy.Api.Models
{
    public class AccountGroupRoot
    {
        public Meta Meta { get; set; }
        public AccountGroup AccountGroup { get; set; }
        public List<AccountGroup> AccountGroups { get; set; }
    }

    public class AccountGroup : IEntity
    { 
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public int AccountNo { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string NatureId { get; set; }
        public string SumFrom { get; set; }
        public string Style { get; set; }
        public int Priority { get; set; }
        public string IntervalFrom { get; set; }
        public string IntervalTo { get; set; }
        public bool AllowPaymentAmounts { get; set; }
        public string PredefinedAccountGroupId { get; set; }
    }
}
