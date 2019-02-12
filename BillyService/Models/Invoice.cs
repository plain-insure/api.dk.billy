using Billy.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillyService.Models
{
    public class Invoice
    {
        public string id { get; set; }
        public string organizationId { get; set; }
        public string type { get; set; }
        public DateTime createdTime { get; set; }
        public DateTime approvedTime { get; set; }
        public string contactId { get; set; }
        public object attContactPersonId { get; set; }

        [JsonConverter(typeof (Converters.BillyDateConverter))]
        public DateTime entryDate { get; set; }
        public object paymentTermsMode { get; set; }
        public object paymentTermsDays { get; set; }

        [JsonConverter(typeof(Converters.BillyDateConverter))]
        public DateTime dueDate { get; set; }
        public string state { get; set; }
        public string sentState { get; set; }
        public string invoiceNo { get; set; }
        public string taxMode { get; set; }
        public double amount { get; set; }
        public double tax { get; set; }
        public string currencyId { get; set; }
        public int exchangeRate { get; set; }
        public int balance { get; set; }
        public bool isPaid { get; set; }
        public object creditedInvoiceId { get; set; }
        public string contactMessage { get; set; }
        public string lineDescription { get; set; }
        public string bankAccountId { get; set; }
        public object recurringInvoiceId { get; set; }
        public string downloadUrl { get; set; }
        public List<InvoiceLine> lines { get; set; }
    }

    public class InvoiceRoot
    {
        public Meta meta { get; set; }
        public Invoice invoice { get; set; }
        public List<Invoice> invoices { get; set; }
        public List<InvoiceLine> invoiceLines { get; set; }
        public List<Organization> organizations { get; set; }
        public List<Transaction> transactions { get; set; }
        public List<Posting> postings { get; set; }
    }
}
