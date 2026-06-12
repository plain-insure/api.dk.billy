namespace Billy.Api.Models
{
    /// <summary>
    /// API response envelope for tax rate endpoints (<c>GET /v2/taxRates</c>, etc.).
    /// </summary>
    public class TaxRateRoot : Root
    {
        /// <summary>Single tax rate returned by a Get request.</summary>
        public TaxRate? TaxRate { get; set; }

        /// <summary>List of tax rates returned by a List request.</summary>
        public List<TaxRate>? TaxRates { get; set; }
    }

    /// <summary>
    /// Represents a tax rate in Billy, used on invoice and bill lines to calculate tax amounts.
    /// Tax rates are read-only through this library — manage them in the Billy web app.
    /// </summary>
    public class TaxRate : IEntity
    {
        /// <summary>Unique identifier assigned by the Billy API.</summary>
        public string? Id { get; set; }

        /// <summary>ID of the organization this tax rate belongs to.</summary>
        public string? OrganizationId { get; set; }

        /// <summary><c>true</c> if this is a Billy system-defined tax rate (not user-created).</summary>
        public bool IsPredefined { get; set; }

        /// <summary>Internal tag identifying a predefined tax rate across organizations (e.g. <c>"vat25"</c>).</summary>
        public string? PredefinedTag { get; set; }

        /// <summary>Display name of the tax rate (e.g. <c>"Danish VAT"</c>).</summary>
        public string? Name { get; set; }

        /// <summary>Short abbreviation shown on invoices and reports (e.g. <c>"DK25"</c>).</summary>
        public string? Abbreviation { get; set; }

        /// <summary>Detailed description of when this tax rate applies.</summary>
        public string? Description { get; set; }

        /// <summary>Tax rate as a decimal fraction (e.g. <c>0.25</c> for 25% VAT).</summary>
        public decimal Rate { get; set; }

        /// <summary><c>true</c> if this tax rate applies to sales (outgoing invoices).</summary>
        public bool AppliesToSales { get; set; }

        /// <summary><c>true</c> if this tax rate applies to purchases (incoming bills).</summary>
        public bool AppliesToPurchases { get; set; }

        /// <summary><c>true</c> if this tax rate is available for use on new transactions.</summary>
        public bool IsActive { get; set; }

        /// <summary>Display order of this tax rate in selection lists (lower = first).</summary>
        public int Priority { get; set; }

        /// <summary>ID of the meta field used to store the net (pre-tax) amount for tax reporting.</summary>
        public string? NetAmountMetaFieldId { get; set; }

        /// <summary>
        /// Deduction components for partially deductible tax rates.
        /// Each component specifies what share of the tax is deductible and to which account.
        /// </summary>
        public List<TaxRateDeductionComponent> DeductionComponents { get; set; } = [];

        /// <summary>
        /// Groups related tax rates together for reporting purposes (e.g. <c>"eu"</c>, <c>"domestic"</c>).
        /// </summary>
        public string? TaxRateGroup { get; set; }

        /// <summary>
        /// <c>true</c> if the organization requires this standard tax rate but it has not yet been configured.
        /// </summary>
        public bool IsMissingStandardTaxRate { get; set; }
    }

    /// <summary>
    /// Defines the deductible portion of a partially deductible <see cref="TaxRate"/>.
    /// Used when only a fraction of the tax paid can be reclaimed (e.g. 50% deductible restaurant VAT).
    /// </summary>
    public class TaxRateDeductionComponent
    {
        /// <summary>Unique identifier of this deduction component.</summary>
        public string? Id { get; set; }

        /// <summary>ID of the <see cref="TaxRate"/> this component belongs to.</summary>
        public string? TaxRateId { get; set; }

        /// <summary>Index position of this component within the tax rate's deduction components list.</summary>
        public int? DeductionComponentIndex { get; set; }

        /// <summary>Fraction of the tax that is deductible (e.g. <c>0.5</c> for 50% deductible).</summary>
        public decimal Share { get; set; }

        /// <summary>Source system that defined this deduction component.</summary>
        public string? Source { get; set; }

        /// <summary>ID of the ledger <see cref="Account"/> to which the deductible portion is posted.</summary>
        public string? AccountId { get; set; }

        /// <summary>Display order of this component within the tax rate's deduction list (lower = first).</summary>
        public int Priority { get; set; }
    }
}
