using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billy.Api.Models
{
    /// <summary>API response envelope for locale list endpoints.</summary>
    public class LocaleRoot
    {
        /// <summary>All locales returned by the request.</summary>
        public IEnumerable<Locale> Locales { get; set; }
    }

    /// <summary>
    /// Represents a locale (language/region combination) supported by the Billy API.
    /// Used to control the language of generated documents such as invoices.
    /// </summary>
    public class Locale : IEntity
    {
        /// <summary>IETF locale tag (e.g. <c>"en_US"</c>, <c>"da_DK"</c>, <c>"de_DE"</c>).</summary>
        public string Id { get; set; }

        /// <summary>Human-readable display name of the locale.</summary>
        public string Name { get; set; }

        /// <summary>URL to the locale's flag icon.</summary>
        public string Icon { get; set; }
    }

}
