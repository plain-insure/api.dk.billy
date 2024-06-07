using Billy.Api.Models;
using System.Diagnostics;

namespace Billy.Api
{
    public class Contacts : Repositories.BaseWithDelete<Contact, ContactRoot>
    {
        public  Contacts(RestSharp.RestClient? client, string? key) : base(
            client, key,
            "contacts/",
            (root) => root.Contact,
            (root) => root.Contacts,
            (item) => item.Id
            )
        {
        
            this.AddSideload(c => c.Countries, c => c.Country);

            //this.AddSideload(c => c.Currencies, c => c.Currency, c => c.CurrencyId);
            this.AddSideload(c => c.Locale);

        }

        public Contacts(RestSharp.RestClient client) : this(client, null) { }

        public Contacts(string key) : this(null, key) { }


    }
}