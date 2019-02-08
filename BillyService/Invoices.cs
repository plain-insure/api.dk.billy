using BillyService.Models;

namespace BillyService
{
    public class Invoices : Repositories.Base<Invoice, InvoiceRoot>
    {
        public Invoices(string key) : base(
            key,
            "invoices/",
            (root) => root.invoice,
            (root) => root.invoices,
            (item) => item.id
            )
        { }


    }
}


