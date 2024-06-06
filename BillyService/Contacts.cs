using BillyService.Models;

namespace BillyService
{
    public class Contacts : Repositories.BaseWithDelete<Contact, ContactRoot>
    {
        public Contacts(RestSharp.RestClient client) : base(
            client,
            "contacts/",
            (root) => root.Contact,
            (root) => root.Contacts,
            (item) => item.Id
            )
        { }

        public Contacts(string key) : base(
            key,
            "contacts/",
            (root) => root.Contact,
            (root) => root.Contacts,
            (item) => item.Id
            )
        { }

    }
}