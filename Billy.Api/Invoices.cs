using Billy.Api.Models;

namespace Billy.Api
{
    public class Invoices : Repositories.BaseWithDelete<Invoice, InvoiceRoot>
    {
        public Invoices(RestSharp.RestClient client) : base(
            client)
        { }

        public Invoices(string key) : base(
            key)
        { }

        public Invoice MostRecent(string contactId)
        {
            var invoices = List(new { contactId }, i => i.ApprovedTime, SortOrder.DESC, 1);
            return invoices.FirstOrDefault();
        }

    }
}


