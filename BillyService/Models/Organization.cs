using System;

namespace Billy.Models
{
    public class Organization
    {
        public string id { get; set; }
        public string ownerUserId { get; set; }
        public DateTime createdTime { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string street { get; set; }
        public string zipcode { get; set; }
        public string city { get; set; }
        public string countryId { get; set; }
        public object stateId { get; set; }
        public string region { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string email { get; set; }
        public string registrationNo { get; set; }
        public string baseCurrencyId { get; set; }
        public object logoFileId { get; set; }
        public object logoPdfFileId { get; set; }
        public object logoUrl { get; set; }
        public object iconFileId { get; set; }
        public object iconUrl { get; set; }
        public object icon48Url { get; set; }
        public int fiscalYearEndMonth { get; set; }
        public string firstFiscalYearStart { get; set; }
        public string firstFiscalYearEnd { get; set; }
        public bool hasBillVoucherNo { get; set; }
        public bool isBillyFree { get; set; }
        public object subscriptionCardType { get; set; }
        public object subscriptionCardNumber { get; set; }
        public object subscriptionCardExpires { get; set; }
        public bool isSubscriptionBankPayer { get; set; }
        public string subscriptionCurrencyId { get; set; }
        public int subscriptionPrice { get; set; }
        public string subscriptionPeriod { get; set; }
        public int subscriptionDiscount { get; set; }
        public string subscriptionExpires { get; set; }
        public bool isTrial { get; set; }
        public bool isTerminated { get; set; }
        public DateTime terminationTime { get; set; }
        public string localeId { get; set; }
        public string billEmailAddress { get; set; }
        public bool isUnmigrated { get; set; }
        public bool isLocked { get; set; }
        public object lockedCode { get; set; }
        public string lockedReason { get; set; }
        public string appUrl { get; set; }
        public string emailAttachmentDeliveryMode { get; set; }
        public bool hasVat { get; set; }
        public string salesTaxPeriod { get; set; }
        public string defaultInvoiceBankAccountId { get; set; }
        public string invoiceNoMode { get; set; }
        public int nextInvoiceNo { get; set; }
        public string paymentTermsMode { get; set; }
        public int paymentTermsDays { get; set; }
        public string defaultSalesAccountId { get; set; }
        public string defaultSalesTaxRulesetId { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(BillyService.Converters.BillyDateConverter))]
        public DateTime bankSyncStartDate { get; set; }
        public string defaultBankFeeAccountId { get; set; }
        public string defaultBillBankAccountId { get; set; }
    }
}
