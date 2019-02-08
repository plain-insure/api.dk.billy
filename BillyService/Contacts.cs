using BillyService.Models;

namespace BillyService
{
    public class Contacts : Repositories.BaseWithDelete<Contact, ContactRoot>
    {
        public Contacts(string key) : base(
            key,
            "contacts/",
            (root) => root.contact,
            (root) => root.contacts,
            (item) => item.id
            )
        { }

    }
}