using Billy.Models;
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
        public string type { get; }
        public DateTime createdTime { get; }
        public DateTime approvedTime { get; }
        public string contactId { get; set; }
        public object attContactPersonId { get; }
        public string entryDate { get; set; }
        public object paymentTermsMode { get; }
        public object paymentTermsDays { get; set; }
        public DateTime dueDate { get; }
        public string state { get; set; }
        public string sentState { get; set; }
        public string invoiceNo { get; set; }
        public string taxMode { get; set; }
        public double amount { get; }
        public double tax { get; }
        public string currencyId { get; }
        public int exchangeRate { get; }
        public int balance { get; }
        public bool isPaid { get; }
        public object creditedInvoiceId { get; }
        public string contactMessage { get; }
        public string lineDescription { get; }
        public string bankAccountId { get; }
        public object recurringInvoiceId { get; }
        public string downloadUrl { get; }
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
