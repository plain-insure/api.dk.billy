using Billy.Api.Actions;
using Billy.Api.Models;
using RestSharp;

namespace Billy.Api.Tests
{
    [TestClass]
    public class BankPaymentActionsTest : TestBase<BankPayments>
    {
        public override BankPayments CreateService(RestClient client) => new(client);

        private Account cashAccount = default!;
        private string expenseAccountId = default!;
        private string purchaseTaxRateId = default!;
        private string customerContactId = default!;
        private string supplierContactId = default!;
        private string productId = default!;

        [TestInitialize]
        public void InitializeTestData()
        {
            var accounts = new Accounts(Client).List()
                ?? throw new InvalidOperationException("Failed to list accounts");

            cashAccount = accounts.FirstOrDefault(a => a.IsPaymentEnabled)
                ?? throw new InvalidOperationException("No payment-enabled account found");

            expenseAccountId = accounts.FirstOrDefault(a => a.AccountNo >= 1200 && a.AccountNo <= 1299)?.Id
                ?? throw new InvalidOperationException("No account found with account number between 1200 and 1299");

            purchaseTaxRateId = new TaxRates(Client).List()
                ?.FirstOrDefault(t => t.AppliesToPurchases && t.IsActive)?.Id
                ?? throw new InvalidOperationException("No active purchase tax rate found in the organisation");

            customerContactId = new Contacts(Client).List(new { isCustomer = true })
                ?.FirstOrDefault()?.Id
                ?? throw new InvalidOperationException("No customer contact found in the organisation");

            supplierContactId = new Contacts(Client).List(new { isSupplier = true })
                ?.FirstOrDefault()?.Id
                ?? throw new InvalidOperationException("No supplier contact found in the organisation");

            productId = new Products(Client).List()
                ?.FirstOrDefault()?.Id
                ?? throw new InvalidOperationException("No products found in the organisation");
        }

        private Invoice GetInvoice(string id) =>
            new Invoices(Client).Get(id)
                ?? throw new InvalidOperationException($"Invoice {id} not found");

        private Bill GetBill(string id) =>
            new Bills(Client).Get(id)
                ?? throw new InvalidOperationException($"Bill {id} not found");

        private void VoidPayment(string id) =>
            service.Update(id, new DeltaObject<BankPayment>().Set(p => p.IsVoided, true));

        private string CreateApprovedInvoice() =>
            new Invoices(Client).Create(new Invoice
            {
                OrganizationId = OrganizationId,
                ContactId = customerContactId,
                EntryDate = DateTime.Now,
                PaymentTermsDays = 0,
                State = "approved",
                SentState = "unsent",
                TaxMode = "incl",
                Lines = [new InvoiceLine { ProductId = productId, UnitPrice = 100, Description = "Test line" }]
            }) ?? throw new InvalidOperationException("Failed to create invoice");

        private string CreateApprovedBill() =>
            new Bills(Client).Create(new Bill
            {
                OrganizationId = OrganizationId,
                ContactId = supplierContactId,
                EntryDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14),
                State = BillStates.approved,
                TaxMode = "incl",
                Lines = [new BillLine { AccountId = expenseAccountId, TaxRateId = purchaseTaxRateId, Amount = 100, Description = "Test line" }]
            }) ?? throw new InvalidOperationException("Failed to create bill");

        // ── PayInvoice ───────────────────────────────────────────────────────────

        [TestMethod]
        public void PayInvoice()
        {
            var invoice = GetInvoice(CreateApprovedInvoice());
            var id = service.PayInvoice(invoice, cashAccount);
            if (id != null) VoidPayment(id);
            Assert.IsNotNull(id);
        }

        [TestMethod]
        public async Task PayInvoiceAsync()
        {
            var invoice = GetInvoice(CreateApprovedInvoice());
            var id = await service.PayInvoiceAsync(invoice, cashAccount);
            if (id != null) VoidPayment(id);
            Assert.IsNotNull(id);
        }

        [TestMethod]
        public void PayInvoice_PartialAmount()
        {
            var invoice = GetInvoice(CreateApprovedInvoice());
            var id = service.PayInvoice(invoice, cashAccount, amount: 50.00);
            if (id != null) VoidPayment(id);
            Assert.IsNotNull(id);
        }

        // ── PayBill ──────────────────────────────────────────────────────────────

        [TestMethod]
        public void PayBill()
        {
            var bill = GetBill(CreateApprovedBill());
            var id = service.PayBill(bill, cashAccount);
            if (id != null) VoidPayment(id);
            Assert.IsNotNull(id);
        }

        [TestMethod]
        public async Task PayBillAsync()
        {
            var bill = GetBill(CreateApprovedBill());
            var id = await service.PayBillAsync(bill, cashAccount);
            if (id != null) VoidPayment(id);
            Assert.IsNotNull(id);
        }

        [TestMethod]
        public void PayBill_PartialAmount()
        {
            var bill = GetBill(CreateApprovedBill());
            var id = service.PayBill(bill, cashAccount, amount: 50.00);
            if (id != null) VoidPayment(id);
            Assert.IsNotNull(id);
        }
    }
}
