﻿using System.Text.Json.Serialization;

namespace Billy.Models
{
    public class Posting
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string TransactionId { get; set; }

        [JsonConverter(typeof(BillyService.Converters.BillyDateConverter))]
        public DateTime EntryDate { get; set; }
        public string Text { get; set; }
        public string AccountId { get; set; }
        public double Amount { get; set; }
        public string Side { get; set; }
        public string CurrencyId { get; set; }
        public string SalesTaxReturnId { get; set; }
        public bool IsVoided { get; set; }
        public bool IsBankMatched { get; set; }
        public int Priority { get; set; }
    }
}