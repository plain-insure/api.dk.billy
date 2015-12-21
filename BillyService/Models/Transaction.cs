using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billy.Models
{
    public class Transaction
    {
        public string id { get; set; }
        public string organizationId { get; set; }
        public int transactionNo { get; set; }
        public string voucherNo { get; set; }
        public string createdTime { get; set; }
        public string entryDate { get; set; }
        public string originatorReference { get; set; }
        public string originatorName { get; set; }
        public bool isVoided { get; set; }
        public bool isVoid { get; set; }
    }
}
