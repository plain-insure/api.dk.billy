using BillyService.Models;
using System.Linq;

namespace BillyService
{
    public class Invoices : Repositories.Base<Invoice, InvoiceRoot>
    {
        public Invoices(RestSharp.RestClient client) : base(
            client,
            "invoices/",
            (root) => root.invoice,
            (root) => root.invoices,
            (item) => item.id
            )
        { }

        public Invoices(string key) : base(
            key,
            "invoices/",
            (root) => root.invoice,
            (root) => root.invoices,
            (item) => item.id
            )
        { }

        public Invoice MostRecent(string contactId)
        {
            var invoices  = List(new { contactId }, i => i.approvedTime, SortOrder.DESC, 1);
            return invoices.FirstOrDefault();
        }

    }
}


