using System.Text.Json.Serialization;

namespace Billy.Api.Models
{
    /// <summary>
    /// API response envelope for organization endpoints (<c>GET /v2/organizations</c>, etc.).
    /// </summary>
    public class OrganizationRoot : Root
    {
        /// <summary>Response metadata.</summary>
        public Meta Meta { get; set; }

        /// <summary>Single organization returned by a Get request.</summary>
        public Organization Organization { get; set; }

        /// <summary>List of organizations the authenticated user has access to.</summary>
        public List<Organization> Organizations { get; set; }
    }

    /// <summary>
    /// Represents a Billy organization (company). Each API access token is scoped to a single organization.
    /// Organizations are read-only through this library.
    /// </summary>
    public class Organization : IEntity
    {
        /// <summary>Unique identifier assigned by the Billy API.</summary>
        public string Id { get; set; }

        /// <summary>ID of the user who owns this organization.</summary>
        public string OwnerUserId { get; set; }

        /// <summary>Timestamp when the organization was created in Billy.</summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>Legal or trading name of the company.</summary>
        public string Name { get; set; }

        /// <summary>Company website URL.</summary>
        public string Url { get; set; }

        /// <summary>Registered street address of the company.</summary>
        public string Street { get; set; }

        /// <summary>Postal/zip code of the company address.</summary>
        public string Zipcode { get; set; }

        /// <summary>City of the company address.</summary>
        public string City { get; set; }

        /// <summary>ISO 3166-1 alpha-2 country code of the company (e.g. <c>"DK"</c>).</summary>
        public string CountryId { get; set; }

        /// <summary>State/region identifier, used in countries that have states.</summary>
        public object StateId { get; set; }

        /// <summary>Region name of the company address.</summary>
        public string Region { get; set; }

        /// <summary>Primary phone number of the company.</summary>
        public string Phone { get; set; }

        /// <summary>Fax number of the company.</summary>
        public string Fax { get; set; }

        /// <summary>Primary email address of the company.</summary>
        public string Email { get; set; }

        /// <summary>
        /// Company registration number (e.g. CVR number in Denmark, VAT number elsewhere).
        /// Used on invoices and for tax reporting.
        /// </summary>
        public string RegistrationNo { get; set; }

        /// <summary>ISO 4217 code of the organization's base/accounting currency (e.g. <c>"DKK"</c>).</summary>
        public string BaseCurrencyId { get; set; }

        /// <summary>ID of the uploaded logo file shown on invoices.</summary>
        public object LogoFileId { get; set; }

        /// <summary>ID of the PDF-optimized logo file.</summary>
        public object LogoPdfFileId { get; set; }

        /// <summary>URL to the organization's logo image.</summary>
        public object LogoUrl { get; set; }

        /// <summary>ID of the organization's icon file.</summary>
        public object IconFileId { get; set; }

        /// <summary>URL to the organization's icon image.</summary>
        public object IconUrl { get; set; }

        /// <summary>URL to a 48×48 px version of the organization's icon.</summary>
        public object Icon48Url { get; set; }

        /// <summary>Month (1–12) on which the fiscal year ends (e.g. <c>12</c> for December).</summary>
        public int FiscalYearEndMonth { get; set; }

        /// <summary>Start date of the organization's first fiscal year.</summary>
        public string FirstFiscalYearStart { get; set; }

        /// <summary>End date of the organization's first fiscal year.</summary>
        public string FirstFiscalYearEnd { get; set; }

        /// <summary><c>true</c> if bills are assigned an internal voucher number by Billy.</summary>
        public bool HasBillVoucherNo { get; set; }

        /// <summary><c>true</c> if the organization is on the free Billy plan.</summary>
        public bool IsBillyFree { get; set; }

        /// <summary>Card type used for subscription payment (e.g. <c>"visa"</c>).</summary>
        public object SubscriptionCardType { get; set; }

        /// <summary>Masked subscription payment card number.</summary>
        public object SubscriptionCardNumber { get; set; }

        /// <summary>Expiry date of the subscription payment card.</summary>
        public object SubscriptionCardExpires { get; set; }

        /// <summary><c>true</c> if the subscription is paid via bank transfer rather than card.</summary>
        public bool IsSubscriptionBankPayer { get; set; }

        /// <summary>ISO 4217 currency code used for billing the Billy subscription.</summary>
        public string SubscriptionCurrencyId { get; set; }

        /// <summary>Monthly subscription price in the subscription currency.</summary>
        public int SubscriptionPrice { get; set; }

        /// <summary>Subscription billing period (e.g. <c>"monthly"</c>, <c>"yearly"</c>).</summary>
        public string SubscriptionPeriod { get; set; }

        /// <summary>Percentage discount applied to the subscription price.</summary>
        public int SubscriptionDiscount { get; set; }

        /// <summary>Date on which the current subscription period expires.</summary>
        public string SubscriptionExpires { get; set; }

        /// <summary><c>true</c> if the organization is on a trial subscription.</summary>
        public bool IsTrial { get; set; }

        /// <summary><c>true</c> if the organization's Billy subscription has been terminated.</summary>
        public bool IsTerminated { get; set; }

        /// <summary>Timestamp when the organization was terminated, or <c>null</c> if still active.</summary>
        public DateTime? TerminationTime { get; set; }

        /// <summary>IETF locale tag controlling the default language for generated documents.</summary>
        public string LocaleId { get; set; }

        /// <summary>Email address to which suppliers can send digital bills for automatic import.</summary>
        public string BillEmailAddress { get; set; }

        /// <summary><c>true</c> if the organization has not yet been migrated to the current Billy platform.</summary>
        public bool IsUnmigrated { get; set; }

        /// <summary><c>true</c> if the organization is locked and cannot create new transactions.</summary>
        public bool IsLocked { get; set; }

        /// <summary>Machine-readable lock reason code.</summary>
        public object LockedCode { get; set; }

        /// <summary>Human-readable explanation of why the organization is locked.</summary>
        public string LockedReason { get; set; }

        /// <summary>URL to the organization's Billy web app workspace.</summary>
        public string AppUrl { get; set; }

        /// <summary>
        /// Default email attachment delivery mode: <c>"link"</c> or <c>"attachment"</c>.
        /// Inherited by new contacts unless overridden on the contact itself.
        /// </summary>
        public string EmailAttachmentDeliveryMode { get; set; }

        /// <summary><c>true</c> if the organization is registered for VAT/sales tax.</summary>
        public bool HasVat { get; set; }

        /// <summary>VAT reporting period (e.g. <c>"monthly"</c>, <c>"quarterly"</c>).</summary>
        public string SalesTaxPeriod { get; set; }

        /// <summary>ID of the default bank <see cref="Account"/> printed on outgoing invoices.</summary>
        public string DefaultInvoiceBankAccountId { get; set; }

        /// <summary>
        /// Invoice numbering mode: <c>"auto"</c> for sequential auto-increment,
        /// or <c>"manual"</c> for user-specified numbers.
        /// </summary>
        public string InvoiceNoMode { get; set; }

        /// <summary>The next invoice number that will be assigned when <see cref="InvoiceNoMode"/> is <c>"auto"</c>.</summary>
        public int NextInvoiceNo { get; set; }

        /// <summary>Default payment terms mode applied to new contacts and invoices.</summary>
        public string PaymentTermsMode { get; set; }

        /// <summary>Default number of payment days applied together with <see cref="PaymentTermsMode"/>.</summary>
        public int PaymentTermsDays { get; set; }

        /// <summary>ID of the default sales <see cref="Account"/> suggested when creating invoice lines.</summary>
        public string DefaultSalesAccountId { get; set; }

        /// <summary>ID of the default <see cref="SalesTaxRuleset"/> applied to new invoices.</summary>
        public string DefaultSalesTaxRulesetId { get; set; }

        /// <summary>
        /// Start date for bank synchronization import (date-only, serialized as <c>yyyy-MM-dd</c>).
        /// Transactions before this date are excluded from bank sync.
        /// </summary>
        [JsonConverter(typeof(Billy.Api.Converters.BillyDateConverter))]
        public DateTime BankSyncStartDate { get; set; }

        /// <summary>ID of the default <see cref="Account"/> used to post bank fees.</summary>
        public string DefaultBankFeeAccountId { get; set; }

        /// <summary>ID of the default bank <see cref="Account"/> used when paying supplier bills.</summary>
        public string DefaultBillBankAccountId { get; set; }
    }
}
