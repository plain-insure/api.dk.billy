namespace Billy.Api.Models
{
    public class TaxRateIdList
    {
        public List<string>? TaxRates { get; set; }
    }

    public class TaxRateRoot : Root<TaxRateIdList>
    {
        public TaxRate? TaxRate { get; set; }
        public List<TaxRate>? TaxRates { get; set; }
    }

    public class TaxRate : IEntity
    {
        public string? Id { get; set; }
        public string? OrganizationId { get; set; }
        public bool IsPredefined { get; set; }
        public string? PredefinedTag { get; set; }
        public string? Name { get; set; }
        public string? Abbreviation { get; set; }
        public string? Description { get; set; }
        public double Rate { get; set; }
        public bool AppliesToSales { get; set; }
        public bool AppliesToPurchases { get; set; }
        public bool IsActive { get; set; }
        public int Priority { get; set; }
        public string? NetAmountMetaFieldId { get; set; }
        public List<TaxRateDeductionComponent> DeductionComponents { get; set; } = [];
        public string? TaxRateGroup { get; set; }
        public bool IsMissingStandardTaxRate { get; set; }
    }

    public class TaxRateDeductionComponent
    {
        public string? Id { get; set; }
        public string? TaxRateId { get; set; }
        public int? DeductionComponentIndex { get; set; }
        public double Share { get; set; }
        public string? Source { get; set; }
        public string? AccountId { get; set; }
        public int Priority { get; set; }
    }
}
