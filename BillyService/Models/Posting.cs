using System.Text.Json.Serialization;

namespace Billy.Models
{
    public class Posting
    {
        public string id { get; set; }
        public string organizationId { get; set; }
        public string transactionId { get; set; }

        [JsonConverter(typeof(BillyService.Converters.BillyDateConverter))]
        public DateTime entryDate { get; set; }
        public string text { get; set; }
        public string accountId { get; set; }
        public double amount { get; set; }
        public string side { get; set; }
        public string currencyId { get; set; }
        public string salesTaxReturnId { get; set; }
        public bool isVoided { get; set; }
        public bool isBankMatched { get; set; }
        public int priority { get; set; }
    }
}
