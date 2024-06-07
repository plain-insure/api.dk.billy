using System.Text.Json.Serialization;

namespace Billy.Api.Models
{
    public class Organization : IEntity
    {
        public string Id { get; set; }
        public string OwnerUserId { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Street { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
        public string CountryId { get; set; }
        public object StateId { get; set; }
        public string Region { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string RegistrationNo { get; set; }
        public string BaseCurrencyId { get; set; }
        public object LogoFileId { get; set; }
        public object LogoPdfFileId { get; set; }
        public object LogoUrl { get; set; }
        public object IconFileId { get; set; }
        public object IconUrl { get; set; }
        public object Icon48Url { get; set; }
        public int FiscalYearEndMonth { get; set; }
        public string FirstFiscalYearStart { get; set; }
        public string FirstFiscalYearEnd { get; set; }
        public bool HasBillVoucherNo { get; set; }
        public bool IsBillyFree { get; set; }
        public object SubscriptionCardType { get; set; }
        public object SubscriptionCardNumber { get; set; }
        public object SubscriptionCardExpires { get; set; }
        public bool IsSubscriptionBankPayer { get; set; }
        public string SubscriptionCurrencyId { get; set; }
        public int SubscriptionPrice { get; set; }
        public string SubscriptionPeriod { get; set; }
        public int SubscriptionDiscount { get; set; }
        public string SubscriptionExpires { get; set; }
        public bool IsTrial { get; set; }
        public bool IsTerminated { get; set; }
        public DateTime TerminationTime { get; set; }
        public string LocaleId { get; set; }
        public string BillEmailAddress { get; set; }
        public bool IsUnmigrated { get; set; }
        public bool IsLocked { get; set; }
        public object LockedCode { get; set; }
        public string LockedReason { get; set; }
        public string AppUrl { get; set; }
        public string EmailAttachmentDeliveryMode { get; set; }
        public bool HasVat { get; set; }
        public string SalesTaxPeriod { get; set; }
        public string DefaultInvoiceBankAccountId { get; set; }
        public string InvoiceNoMode { get; set; }
        public int NextInvoiceNo { get; set; }
        public string PaymentTermsMode { get; set; }
        public int PaymentTermsDays { get; set; }
        public string DefaultSalesAccountId { get; set; }
        public string DefaultSalesTaxRulesetId { get; set; }

        [JsonConverter(typeof(Billy.Api.Converters.BillyDateConverter))]
        public DateTime BankSyncStartDate { get; set; }
        public string DefaultBankFeeAccountId { get; set; }
        public string DefaultBillBankAccountId { get; set; }
    }
}
