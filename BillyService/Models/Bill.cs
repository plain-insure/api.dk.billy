using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillyService.Models
{
    public class Bill
    {
        public string id { get; set; }
        public string organizationId { get; set; }
        public string type { get; set; }
        public string createdTime { get; set; }
        public string approvedTime { get; set; }
        public string contactId { get; set; }
        public object contactName { get; set; }
        public string entryDate { get; set; }
        public object paymentAccountId { get; set; }
        public object paymentDate { get; set; }
        public string dueDate { get; set; }
        public bool isBare { get; set; }
        public string state { get; set; }
        public string suppliersInvoiceNo { get; set; }
        public string taxMode { get; set; }
        public string voucherNo { get; set; }
        public int amount { get; set; }
        public int tax { get; set; }
        public string currencyId { get; set; }
        public int exchangeRate { get; set; }
        public int balance { get; set; }
        public bool isPaid { get; set; }
        public string lineDescription { get; set; }
        public object creditedBillId { get; set; }
        public object source { get; set; }
        public object subject { get; set; }
    }

    public class GetBillListRoot
    {
        public Meta meta { get; set; }
        public List<Bill> bills { get; set; }
    }

    public class GetBillRoot
    {
        public Meta meta { get; set; }
        public Bill bill { get; set; }
    }

    public class PostBillRoot
    {
        public PostBill bill { get; set; }
    }

    public class PostBillResultRoot
    {

    }

    public class PostBill
    {
        public string organizationId { get; set; }
        public string contactId { get; set; }
        public string state { get; set; }
        public string entryDate { get; set; }
        public string dueDate { get; set; }
        public string voucherNo { get; set; }
        public string paymentDate { get; set; }
        public string paymentAccountId { get; set; }
        public string taxMode { get; set; }
        public List<BillLine> lines { get; set; }
    }

    public class BillLine
    {
        public string accountId { get; set; }
        public string description { get; set; }
        public float amount { get; set; }
        public string taxRateId { get; set; }
    }
}
