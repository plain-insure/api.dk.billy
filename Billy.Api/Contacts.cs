using Billy.Api.Models;
using System.Diagnostics;

namespace Billy.Api
{
    /// <summary>
    /// Repository for the Billy <c>/v2/contacts</c> endpoint. Supports full CRUD — Get, List, Create,
    /// Update, and Delete. Use <see cref="SideloadCountry"/> and <see cref="SideloadLocale"/> to
    /// populate the corresponding navigation properties in responses.
    /// </summary>
    public class Contacts(RestSharp.RestClient? client, string? key) : Repositories.BaseWithDelete<Contact, ContactRoot>(
        client, key
            )

    {
        /// <inheritdoc cref="Repositories.Base{T,TRoot}(RestSharp.RestClient)"/>
        public Contacts(RestSharp.RestClient client) : this(client, null) { }

        /// <inheritdoc cref="Repositories.Base{T,TRoot}(string)"/>
        public Contacts(string key) : this(null, key) { }

        /// <summary>
        /// Enables sideloading of <see cref="Contact.Country"/> in all subsequent Get and List responses.
        /// Without calling this, <see cref="Contact.Country"/> will be <c>null</c>.
        /// </summary>
        public void SideloadCountry()
        {
            this.AddSideload(c => c.Countries, c => c.Country, "contact.country");
        }

        /// <summary>
        /// Enables sideloading of <see cref="Contact.Locale"/> in all subsequent Get and List responses.
        /// Without calling this, <see cref="Contact.Locale"/> will be <c>null</c>.
        /// </summary>
        public void SideloadLocale()
        {
            this.AddSideload(c => c.Locale, "contact.locale");
        }
    }
}
