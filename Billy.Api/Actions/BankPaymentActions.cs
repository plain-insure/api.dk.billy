using Billy.Api.Models;

namespace Billy.Api.Actions
{
    /// <summary>
    /// Extension methods on <see cref="BankPayments"/> that simplify the common task of marking
    /// invoices and bills as paid with a single method call.
    /// </summary>
    public static class BankPaymentActions
    {
        /// <summary>
        /// Records receipt of an invoice payment by creating a bank payment that debits the specified
        /// cash account and associates it with the invoice, marking it as paid.
        /// </summary>
        /// <param name="payments">The <see cref="BankPayments"/> repository to create the payment through.</param>
        /// <param name="invoice">The invoice to mark as paid.</param>
        /// <param name="cashAccount">The bank or cash account that received the funds.</param>
        /// <param name="entryDate">Value date of the payment; defaults to today when <c>null</c>.</param>
        /// <param name="amount">Amount received; defaults to the invoice's outstanding balance when <c>null</c>.</param>
        /// <returns>The created <see cref="BankPayment"/>, or <c>null</c> if creation failed.</returns>
        public static BankPayment? PayInvoice(
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

        /// <summary>
        /// Records receipt of an invoice payment asynchronously by creating a bank payment that debits
        /// the specified cash account and associates it with the invoice, marking it as paid.
        /// </summary>
        /// <param name="payments">The <see cref="BankPayments"/> repository to create the payment through.</param>
        /// <param name="invoice">The invoice to mark as paid.</param>
        /// <param name="cashAccount">The bank or cash account that received the funds.</param>
        /// <param name="entryDate">Value date of the payment; defaults to today when <c>null</c>.</param>
        /// <param name="amount">Amount received; defaults to the invoice's outstanding balance when <c>null</c>.</param>
        /// <returns>The created <see cref="BankPayment"/>, or <c>null</c> if creation failed.</returns>
        public static Task<BankPayment?> PayInvoiceAsync(
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

        /// <summary>
        /// Records payment of a supplier bill by creating a bank payment that credits the specified
        /// cash account and associates it with the bill, marking it as paid.
        /// </summary>
        /// <param name="payments">The <see cref="BankPayments"/> repository to create the payment through.</param>
        /// <param name="bill">The bill to mark as paid.</param>
        /// <param name="cashAccount">The bank or cash account from which the funds were disbursed.</param>
        /// <param name="entryDate">Value date of the payment; defaults to today when <c>null</c>.</param>
        /// <param name="amount">Amount paid; defaults to the bill's outstanding balance when <c>null</c>.</param>
        /// <returns>The created <see cref="BankPayment"/>, or <c>null</c> if creation failed.</returns>
        public static BankPayment? PayBill(
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

        /// <summary>
        /// Records payment of a supplier bill asynchronously by creating a bank payment that credits
        /// the specified cash account and associates it with the bill, marking it as paid.
        /// </summary>
        /// <param name="payments">The <see cref="BankPayments"/> repository to create the payment through.</param>
        /// <param name="bill">The bill to mark as paid.</param>
        /// <param name="cashAccount">The bank or cash account from which the funds were disbursed.</param>
        /// <param name="entryDate">Value date of the payment; defaults to today when <c>null</c>.</param>
        /// <param name="amount">Amount paid; defaults to the bill's outstanding balance when <c>null</c>.</param>
        /// <returns>The created <see cref="BankPayment"/>, or <c>null</c> if creation failed.</returns>
        public static Task<BankPayment?> PayBillAsync(
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
