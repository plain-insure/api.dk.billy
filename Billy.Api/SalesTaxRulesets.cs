using Billy.Api.Models;

namespace Billy.Api
{
    /// <summary>
    /// Read-only repository for the Billy <c>/v2/salesTaxRulesets</c> endpoint. Supports Get and List.
    /// </summary>
    public class SalesTaxRulesets : Repositories.Base<SalesTaxRuleset, SalesTaxRulesetRoot>
    {
        private SalesTaxRulesets(RestSharp.RestClient? client, string? key) : base(
            client, key
            )
        { }

        /// <inheritdoc cref="Repositories.Base{T,TRoot}(RestSharp.RestClient)"/>
        public SalesTaxRulesets(RestSharp.RestClient client) : this(client, null) { }

        /// <inheritdoc cref="Repositories.Base{T,TRoot}(string)"/>
        public SalesTaxRulesets(string key) : this(null, key) { }
    }
}
