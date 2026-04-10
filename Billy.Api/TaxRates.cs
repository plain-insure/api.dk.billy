using Billy.Api.Models;

namespace Billy.Api
{
    public class TaxRates : Repositories.Base<TaxRate, TaxRateRoot>
    {
        private TaxRates(RestSharp.RestClient? client, string? key) : base(
            client, key,
            "taxRates/",
            (root) => root?.TaxRate,
            (root) => root?.TaxRates,
            (item) => item?.Id
            )
        { }

        public TaxRates(RestSharp.RestClient client) : this(client, null) { }

        public TaxRates(string key) : this(null, key) { }
    }
}
