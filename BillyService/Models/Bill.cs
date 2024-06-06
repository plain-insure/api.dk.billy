using Billy.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BillyService.Models
{
    public class BillRoot
    {
        public Meta Meta { get; set; }
        public Bill Bill { get; set; }
        public List<Bill> Bills { get; set; }
        public List<BillLine> BillLines { get; set; }
        public List<Transaction> Transactions { get; set; }
        public List<Posting> Postings { get; set; }
    }

    public class Bill
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string Type { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ApprovedTime { get; set; }
        public string ContactId { get; set; }
        public object ContactName { get; set; }

        [JsonConverter(typeof(Converters.BillyDateConverter))]
        public DateTime EntryDate { get; set; }

        public string PaymentAccountId { get; set; }

        [JsonConverter(typeof(Converters.BillyDateConverter))]
        public DateTime PaymentDate { get; set; }

        [JsonConverter(typeof(Converters.BillyDateConverter))]
        public DateTime DueDate { get; set; }
        public bool IsBare { get; set; }
        public string State { get; set; }
        public string SuppliersInvoiceNo { get; set; }
        public string TaxMode { get; set; }
        public string VoucherNo { get; set; }
        public double Amount { get; set; }
        public double Tax { get; set; }
        public string CurrencyId { get; set; }
        public int ExchangeRate { get; set; }
        public int Balance { get; set; }
        public bool IsPaid { get; set; }
        public string LineDescription { get; set; }
        public object CreditedBillId { get; set; }
        public object Source { get; set; }
        public object Subject { get; set; }
        public List<BillLine> Lines { get; set; }
    }

    public class BillLine
    {
        public string Id { get; set; }
        public string BillId { get; set; }
        public string AccountId { get; set; }
        public string TaxRateId { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public double Tax { get; set; }
        public int Priority { get; set; }
    }
}
