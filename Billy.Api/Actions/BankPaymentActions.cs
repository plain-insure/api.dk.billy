using Billy.Api.Models;

namespace Billy.Api.Actions
{
    public static class BankPaymentActions
    {
        // Pay an invoice — records an incoming deposit (debit) against the invoice's outstanding balance
        public static string? PayInvoice(
            this BankPayments payments,
            Invoice invoice,
            Account cashAccount,
            DateTime? entryDate = null,
            double? amount = null) =>
            payments.Create(new BankPayment
            {
                OrganizationId = invoice.OrganizationId,
                ContactId = invoice.ContactId,
                EntryDate = entryDate ?? DateTime.Today,
                CashAmount = amount ?? (double)invoice.Balance,
                CashSide = CashSide.debit,
                CashAccountId = cashAccount.Id,
                SubjectCurrencyId = invoice.CurrencyId,
                Associations = [new BankPaymentAssociation { SubjectReference = $"invoice:{invoice.Id}" }]
            });

        // Pay an invoice — records an incoming deposit (debit) against the invoice's outstanding balance
        public static Task<string?> PayInvoiceAsync(
            this BankPayments payments,
            Invoice invoice,
            Account cashAccount,
            DateTime? entryDate = null,
            double? amount = null) =>
            payments.CreateAsync(new BankPayment
            {
                OrganizationId = invoice.OrganizationId,
                ContactId = invoice.ContactId,
                EntryDate = entryDate ?? DateTime.Today,
                CashAmount = amount ?? (double)invoice.Balance,
                CashSide = CashSide.debit,
                CashAccountId = cashAccount.Id,
                SubjectCurrencyId = invoice.CurrencyId,
                Associations = [new BankPaymentAssociation { SubjectReference = $"invoice:{invoice.Id}" }]
            });

        // Pay a bill — records an outgoing withdrawal (credit) against the bill's outstanding balance
        public static string? PayBill(
            this BankPayments payments,
            Bill bill,
            Account cashAccount,
            DateTime? entryDate = null,
            double? amount = null) =>
            payments.Create(new BankPayment
            {
                OrganizationId = bill.OrganizationId,
                ContactId = bill.ContactId,
                EntryDate = entryDate ?? DateTime.Today,
                CashAmount = amount ?? (double)bill.Balance,
                CashSide = CashSide.credit,
                CashAccountId = cashAccount.Id,
                SubjectCurrencyId = bill.CurrencyId,
                Associations = [new BankPaymentAssociation { SubjectReference = $"bill:{bill.Id}" }]
            });

        // Pay a bill — records an outgoing withdrawal (credit) against the bill's outstanding balance
        public static Task<string?> PayBillAsync(
            this BankPayments payments,
            Bill bill,
            Account cashAccount,
            DateTime? entryDate = null,
            double? amount = null) =>
            payments.CreateAsync(new BankPayment
            {
                OrganizationId = bill.OrganizationId,
                ContactId = bill.ContactId,
                EntryDate = entryDate ?? DateTime.Today,
                CashAmount = amount ?? (double)bill.Balance,
                CashSide = CashSide.credit,
                CashAccountId = cashAccount.Id,
                SubjectCurrencyId = bill.CurrencyId,
                Associations = [new BankPaymentAssociation { SubjectReference = $"bill:{bill.Id}" }]
            });
    }
}
