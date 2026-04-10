using Billy.Api.Models;

namespace Billy.Api
{
    public class SalesTaxRulesets : Repositories.Base<SalesTaxRuleset, SalesTaxRulesetRoot>
    {
        private SalesTaxRulesets(RestSharp.RestClient? client, string? key) : base(
            client, key,
            "salesTaxRulesets/",
            (root) => root?.SalesTaxRuleset,
            (root) => root?.SalesTaxRulesets,
            (item) => item?.Id
            )
        { }

        public SalesTaxRulesets(RestSharp.RestClient client) : this(client, null) { }

        public SalesTaxRulesets(string key) : this(null, key) { }
    }
}
