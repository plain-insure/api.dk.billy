namespace Billy.Api.Models
{
    /// <summary>API response envelope for country list endpoints.</summary>
    public class CountryRoot
    {
        /// <summary>All countries returned by the request.</summary>
        public Country[] Countries { get; set; }
    }

    /// <summary>
    /// Represents a country in the Billy API. Countries are used to set the address and tax jurisdiction
    /// of contacts and organizations.
    /// </summary>
    public class Country : IEntity
    {
        /// <summary>ISO 3166-1 alpha-2 country code (e.g. <c>"DK"</c>, <c>"DE"</c>).</summary>
        public string Id { get; set; }

        /// <summary>English display name of the country.</summary>
        public string Name { get; set; }

        /// <summary><c>true</c> if the country has administrative states/regions.</summary>
        public bool HasStates { get; set; }

        /// <summary><c>true</c> if the list of states for this country is finite and API-managed.</summary>
        public bool HasFiniteStates { get; set; }

        /// <summary><c>true</c> if the list of zip codes for this country is finite and API-managed.</summary>
        public bool HasFiniteZipcodes { get; set; }

        /// <summary>URL to the country's flag icon.</summary>
        public string Icon { get; set; }

        /// <summary>ID of the default locale associated with this country.</summary>
        public string LocaleId { get; set; }

        /// <summary>Resolved locale object. Set when sideloaded by the API.</summary>
        public Locale Locale { get; set; }

        /// <summary>ID of the default currency for this country.</summary>
        public string CurrencyId { get; set; }

        /// <summary>Resolved currency object. Set when sideloaded by the API.</summary>
        public Currency Currency { get; set; }
    }

}
