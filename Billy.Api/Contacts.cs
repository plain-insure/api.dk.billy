using Billy.Api.Models;
using System.Diagnostics;

namespace Billy.Api
{
    public class Contacts(RestSharp.RestClient? client, string? key) : Repositories.BaseWithDelete<Contact, ContactRoot>(
        client, key,
        "contacts/",
        (root) => root?.Contact,
        (root) => root?.Contacts,
        (item) => item?.Id,
        (item) => new ContactRoot { Contact = item },
        (root) => root?.Meta?.DeletedRecords?.Contacts?.FirstOrDefault()
            )

    {
        public Contacts(RestSharp.RestClient client) : this(client, null) { }

        public Contacts(string key) : this(null, key) { }

        public void SideloadCountry()
        {
            this.AddSideload(c => c.Countries, c => c.Country, "contact.country");
        }

        public void SideloadLocale()
        {
            this.AddSideload(c => c.Locale, "contact.locale");
        }
    }
}