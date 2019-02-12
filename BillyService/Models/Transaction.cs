using System;

namespace Billy.Models
{
    public class Transaction
    {
        public string id { get; set; }
        public string organizationId { get; set; }
        public int transactionNo { get; set; }
        public string voucherNo { get; set; }
        public DateTime createdTime { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(BillyService.Converters.BillyDateConverter))]
        public DateTime entryDate { get; set; }
        public string originatorReference { get; set; }
        public string originatorName { get; set; }
        public bool isVoided { get; set; }
        public bool isVoid { get; set; }
    }
}
