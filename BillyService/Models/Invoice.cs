using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillyService.Models
{
    public class InvoiceRoot
    {
        public Meta meta { get; set; }
        public Invoice invoice { get; set; }
    }

    public class Meta
    {
        public int statusCode { get; set; }
        public bool success { get; set; }
        public float time { get; set; }
        public Paging paging { get; set; }
    }

    public class Invoice
    {
        public string id { get; set; }
        public string organizationId { get; set; }
        public string type { get; set; }
        public DateTime createdTime { get; set; }
        public DateTime approvedTime { get; set; }
        public string contactId { get; set; }
        public object attContactPersonId { get; set; }
        public string entryDate { get; set; }
        public string paymentTermsMode { get; set; }
        public int paymentTermsDays { get; set; }
        public string dueDate { get; set; }
        public string state { get; set; }
        public string sentState { get; set; }
        public string invoiceNo { get; set; }
        public string taxMode { get; set; }
        public int amount { get; set; }
        public float tax { get; set; }
        public string currencyId { get; set; }
        public int exchangeRate { get; set; }
        public float balance { get; set; }
        public bool isPaid { get; set; }
        public object creditedInvoiceId { get; set; }
        public string contactMessage { get; set; }
        public string lineDescription { get; set; }
        public List<InvoiceLine> lines { get; set; }
        public string bankAccountId { get; set; }
        public string downloadUrl { get; set; }
    }

    public class InvoiceListItem
    {
        public string id { get; set; }
        public string organizationId { get; set; }
        public string type { get; set; }
        public string createdTime { get; set; }
        public string approvedTime { get; set; }
        public string contactId { get; set; }
        public object attContactPersonId { get; set; }
        public string entryDate { get; set; }
        public string paymentTermsMode { get; set; }
        public int? paymentTermsDays { get; set; }
        public string dueDate { get; set; }
        public string state { get; set; }
        public string sentState { get; set; }
        public string invoiceNo { get; set; }
        public string taxMode { get; set; }
        public int amount { get; set; }
        public double tax { get; set; }
        public string currencyId { get; set; }
        public int exchangeRate { get; set; }
        public double balance { get; set; }
        public bool isPaid { get; set; }
        public object creditedInvoiceId { get; set; }
        public string contactMessage { get; set; }
        public string lineDescription { get; set; }
        public string bankAccountId { get; set; }
        public string downloadUrl { get; set; }
    }

    
    public class InvoiceListRoot
    {
        public Meta meta { get; set; }
        public List<InvoiceListItem> invoices { get; set; }
    }

    public class Paging
    {
        public int page { get; set; }
        public int pageSize { get; set; }
        public int pageCount { get; set; }
        public int total { get; set; }
    }

    public class PostInvoiceRoot
    {
        public PostInvoice invoice { get; set; }
    }

    public class PostInvoice
    {
        public string invoiceNo { get; set; }
        public string organizationId { get; set; }
        public string contactId { get; set; }
        public string entryDate { get; set; }
        public string taxMode { get; set; }
        public int paymentTermsDays { get; set; }
        public string state { get; set; }
        public string paymentTermsMode { get; set; }
        public string sentState { get; set; }
        public string contactMessage { get; set; }
        public List<InvoiceLine> lines { get; set; }
    }

    public class InvoiceLine
    {
        public string productId { get; set; }
        public float unitPrice { get; set; }
        public string description { get; set; }
    }

   
    public class Organization
    {
        public string id { get; set; }
        public string ownerUserId { get; set; }
        public string createdTime { get; set; }
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
        public object terminationTime { get; set; }
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
        public object bankSyncStartDate { get; set; }
        public string defaultBankFeeAccountId { get; set; }
        public string defaultBillBankAccountId { get; set; }
    }

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

    public class Posting
    {
        public string id { get; set; }
        public string organizationId { get; set; }
        public string transactionId { get; set; }
        public string entryDate { get; set; }
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

    public class PostInvoiceResultRoot
    {
        public Meta meta { get; set; }
        public List<Invoice> invoices { get; set; }
        public List<InvoiceLine> invoiceLines { get; set; }
        public List<Organization> organizations { get; set; }
        public List<Transaction> transactions { get; set; }
        public List<Posting> postings { get; set; }
    }
}
