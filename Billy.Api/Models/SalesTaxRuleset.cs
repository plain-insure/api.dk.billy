namespace Billy.Api.Models
{
    /// <summary>
    /// API response envelope for sales tax ruleset endpoints (<c>GET /v2/salesTaxRulesets</c>, etc.).
    /// </summary>
    public class SalesTaxRulesetRoot : Root
    {
        /// <summary>Single sales tax ruleset returned by a Get request.</summary>
        public SalesTaxRuleset? SalesTaxRuleset { get; set; }

        /// <summary>List of sales tax rulesets returned by a List request.</summary>
        public List<SalesTaxRuleset>? SalesTaxRulesets { get; set; }
    }

    /// <summary>
    /// Represents a sales tax ruleset in Billy. A ruleset contains an ordered set of rules that
    /// determine which <see cref="TaxRate"/> applies to a transaction based on the customer's country,
    /// state, and contact type. Rulesets are read-only through this library.
    /// </summary>
    public class SalesTaxRuleset : IEntity
    {
        /// <summary>Unique identifier assigned by the Billy API.</summary>
        public string? Id { get; set; }

        /// <summary>ID of the organization this ruleset belongs to.</summary>
        public string? OrganizationId { get; set; }

        /// <summary>Display name of the ruleset (e.g. <c>"Standard Danish VAT"</c>).</summary>
        public string? Name { get; set; }

        /// <summary>Short abbreviation used in lists and reports (e.g. <c>"DK-VAT"</c>).</summary>
        public string? Abbreviation { get; set; }

        /// <summary>Description explaining when and how this ruleset should be used.</summary>
        public string? Description { get; set; }

        /// <summary>
        /// ID of the <see cref="TaxRate"/> applied when no rule in <see cref="Rules"/> matches
        /// the customer's location or type.
        /// </summary>
        public string? FallbackTaxRateId { get; set; }

        /// <summary><c>true</c> if this ruleset is a Billy system-defined ruleset (not user-created).</summary>
        public bool IsPredefined { get; set; }

        /// <summary>
        /// Ordered list of rules that map geographic or contact-type criteria to a specific tax rate.
        /// Rules are evaluated in <see cref="SalesTaxRulesetRule.Priority"/> order; the first match wins.
        /// Rules are immutable — they must be managed through the Billy web app.
        /// </summary>
        public List<SalesTaxRulesetRule> Rules { get; set; } = [];

        /// <summary>Internal tag identifying a predefined ruleset across organizations.</summary>
        public string? PredefinedTag { get; set; }
    }

    /// <summary>
    /// A single matching rule within a <see cref="SalesTaxRuleset"/>.
    /// When a customer matches all specified criteria, the associated <see cref="TaxRateId"/> is applied.
    /// </summary>
    public class SalesTaxRulesetRule
    {
        /// <summary>Unique identifier of this rule.</summary>
        public string? Id { get; set; }

        /// <summary>ID of the <see cref="SalesTaxRuleset"/> this rule belongs to.</summary>
        public string? RulesetId { get; set; }

        /// <summary>
        /// ISO 3166-1 alpha-2 country code that the customer must match for this rule to apply.
        /// <c>null</c> means the rule applies to any country.
        /// </summary>
        public string? CountryId { get; set; }

        /// <summary>State identifier that the customer must match, or <c>null</c> for any state.</summary>
        public string? StateId { get; set; }

        /// <summary>City identifier that the customer must match, or <c>null</c> for any city.</summary>
        public string? CityId { get; set; }

        /// <summary>Country group identifier (e.g. <c>"EU"</c>) the customer must belong to, or <c>null</c>.</summary>
        public string? CountryGroupId { get; set; }

        /// <summary>
        /// Contact type that must match: <c>"company"</c>, <c>"person"</c>, or <c>null</c> for any type.
        /// </summary>
        public string? ContactType { get; set; }

        /// <summary>ID of the <see cref="TaxRate"/> applied when this rule matches the customer.</summary>
        public string? TaxRateId { get; set; }

        /// <summary>Evaluation order; lower values are checked first. The first matching rule wins.</summary>
        public int Priority { get; set; }
    }
}
