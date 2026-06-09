using Billy.Api.Models;

namespace Billy.Api
{
    /// <summary>
    /// Read-only repository for the Billy <c>/v2/taxRates</c> endpoint. Supports Get and List.
    /// </summary>
    public class TaxRates : Repositories.Base<TaxRate, TaxRateRoot>
    {
        private TaxRates(RestSharp.RestClient? client, string? key) : base(
            client, key
            )
        { }

        /// <inheritdoc cref="Repositories.Base{T,TRoot}(RestSharp.RestClient)"/>
        public TaxRates(RestSharp.RestClient client) : this(client, null) { }

        /// <inheritdoc cref="Repositories.Base{T,TRoot}(string)"/>
        public TaxRates(string key) : this(null, key) { }
    }
}
