namespace Billy.Api.Models
{
    public class SalesTaxRulesetRoot : Root
    {
        public SalesTaxRuleset? SalesTaxRuleset { get; set; }
        public List<SalesTaxRuleset>? SalesTaxRulesets { get; set; }
    }

    public class SalesTaxRuleset : IEntity
    {
        public string? Id { get; set; }
        public string? OrganizationId { get; set; }
        public string? Name { get; set; }
        public string? Abbreviation { get; set; }
        public string? Description { get; set; }
        public string? FallbackTaxRateId { get; set; }
        public bool IsPredefined { get; set; }
        public List<SalesTaxRulesetRule> Rules { get; set; } = [];
        public string? PredefinedTag { get; set; }
    }

    public class SalesTaxRulesetRule
    {
        public string? Id { get; set; }
        public string? RulesetId { get; set; }
        public string? CountryId { get; set; }
        public string? StateId { get; set; }
        public string? CityId { get; set; }
        public string? CountryGroupId { get; set; }
        public string? ContactType { get; set; }
        public string? TaxRateId { get; set; }
        public int Priority { get; set; }
    }
}
