using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billy.Models
{
    public class InvoiceLine
    {
        public string id { get; set; }
        public string invoiceId { get; set; }
        public string productId { get; set; }
        public string description { get; set; }
        public int quantity { get; set; }
        public int unitPrice { get; set; }
        public int amount { get; set; }
        public double tax { get; set; }
        public string taxRateId { get; set; }
        public object discountText { get; set; }
        public object discountMode { get; set; }
        public object discountValue { get; set; }
        public int priority { get; set; }
    }
}
