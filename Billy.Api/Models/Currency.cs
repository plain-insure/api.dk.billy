using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billy.Api.Models
{
    /// <summary>
    /// Represents a currency supported by the Billy API.
    /// Currency objects are sideloaded by some endpoints rather than returned as a separate resource.
    /// </summary>
    public class Currency
    {
        /// <summary>ISO 4217 currency code (e.g. <c>"DKK"</c>, <c>"EUR"</c>, <c>"USD"</c>).</summary>
        public string Id { get; set; }

        /// <summary>Human-readable display name of the currency.</summary>
        public string Name { get; set; }

        /// <summary>Exchange rate relative to the organization's base currency.</summary>
        public float ExchangeRate { get; set; }

        /// <summary>Timestamp when this currency was deleted, or <c>null</c> if still active.</summary>
        public object DeletedTime { get; set; }
    }

}
