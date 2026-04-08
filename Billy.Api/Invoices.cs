using Billy.Api.Models;
using System.Linq;

namespace Billy.Api
{
    public class Invoices : Repositories.Base<Invoice, InvoiceRoot>
    {
        public Invoices(RestSharp.RestClient client) : base(
            client,
            "invoices/",
            (root) => root.Invoice,
            (root) => root.Invoices,
            (item) => item.Id,
            (item) => new InvoiceRoot { Invoice = item }
            )
        { }

        public Invoices(string key) : base(
            key,
            "invoices/",
            (root) => root.Invoice,
            (root) => root.Invoices,
            (item) => item.Id,
            (item) => new InvoiceRoot { Invoice = item }
            )
        { }

        public Invoice MostRecent(string contactId)
        {
            var invoices  = List(new { contactId }, i => i.ApprovedTime, SortOrder.DESC, 1);
            return invoices.FirstOrDefault();
        }

    }
}


