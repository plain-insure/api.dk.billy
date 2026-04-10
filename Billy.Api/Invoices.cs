using Billy.Api.Models;

namespace Billy.Api
{
    public class Invoices : Repositories.BaseWithDelete<Invoice, InvoiceRoot>
    {
        public Invoices(RestSharp.RestClient client) : base(
            client,
            "invoices/",
            (root) => root.Invoice,
            (root) => root.Invoices,
            (item) => item.Id,
            (item) => new InvoiceRoot { Invoice = item },
                        (root) => root.Invoice?.Id
            )
        { }

        public Invoices(string key) : base(
            key,
            "invoices/",
            (root) => root.Invoice,
            (root) => root.Invoices,
            (item) => item.Id,
            (item) => new InvoiceRoot { Invoice = item },
                        (root) => root.Invoice?.Id
            )
        { }

        public Invoice MostRecent(string contactId)
        {
            var invoices = List(new { contactId }, i => i.ApprovedTime, SortOrder.DESC, 1);
            return invoices.FirstOrDefault();
        }

    }
}


