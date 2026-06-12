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

            var contacts = new Contacts(Client);

            customerContactId = contacts.List(new { isCustomer = true })?.FirstOrDefault()?.Id
                ?? contacts.Create(new Contact
                {
                    Type = "company",
                    OrganizationId = OrganizationId,
                    Name = "Test Customer",
                    CountryId = Countries.DK.ToString(),
                    IsCustomer = true
                })?.Id
                ?? throw new InvalidOperationException("Failed to find or create a customer contact");

            supplierContactId = contacts.List(new { isSupplier = true })?.FirstOrDefault()?.Id
                ?? contacts.Create(new Contact
                {
                    Type = "company",
                    OrganizationId = OrganizationId,
                    Name = "Test Supplier",
                    CountryId = Countries.DK.ToString(),
                    IsSupplier = true
                })?.Id
                ?? throw new InvalidOperationException("Failed to find or create a supplier contact");

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
                EntryDate = DateOnly.FromDateTime(DateTime.Now),
                PaymentTermsDays = 0,
                State = State.Approved,
                SentState = SentState.Unsent,
                TaxMode = TaxMode.Excl,
                Lines = [new InvoiceLine { ProductId = productId, UnitPrice = 100, Description = "Test line" }]
            })?.Id ?? throw new InvalidOperationException("Failed to create invoice");

        private string CreateApprovedBill() =>
            new Bills(Client).Create(new Bill
            {
                OrganizationId = OrganizationId,
                ContactId = supplierContactId,
                EntryDate = DateOnly.FromDateTime(DateTime.Now),
                DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(14)),
                State = State.Approved,
                TaxMode = TaxMode.Incl,
                Lines = [new BillLine { AccountId = expenseAccountId, TaxRateId = purchaseTaxRateId, Amount = 100, Description = "Test line" }]
            })?.Id ?? throw new InvalidOperationException("Failed to create bill");

        // ── PayInvoice ───────────────────────────────────────────────────────────

        [TestMethod]
        public void PayInvoice()
        {
            var invoice = GetInvoice(CreateApprovedInvoice());
            var payment = service.PayInvoice(invoice, cashAccount);
            if (payment?.Id != null) VoidPayment(payment.Id);
            Assert.IsNotNull(payment);
        }

        [TestMethod]
        public async Task PayInvoiceAsync()
        {
            var invoice = GetInvoice(CreateApprovedInvoice());
            var payment = await service.PayInvoiceAsync(invoice, cashAccount);
            if (payment?.Id != null) VoidPayment(payment.Id);
            Assert.IsNotNull(payment);
        }

        [TestMethod]
        public void PayInvoice_PartialAmount()
        {
            var invoice = GetInvoice(CreateApprovedInvoice());
            var payment = service.PayInvoice(invoice, cashAccount, amount: 50.00m);
            if (payment?.Id != null) VoidPayment(payment.Id);
            Assert.IsNotNull(payment);
        }

        // ── PayBill ──────────────────────────────────────────────────────────────

        [TestMethod]
        public void PayBill()
        {
            var bill = GetBill(CreateApprovedBill());
            var payment = service.PayBill(bill, cashAccount);
            if (payment?.Id != null) VoidPayment(payment.Id);
            Assert.IsNotNull(payment);
        }

        [TestMethod]
        public async Task PayBillAsync()
        {
            var bill = GetBill(CreateApprovedBill());
            var payment = await service.PayBillAsync(bill, cashAccount);
            if (payment?.Id != null) VoidPayment(payment.Id);
            Assert.IsNotNull(payment);
        }

        [TestMethod]
        public void PayBill_PartialAmount()
        {
            var bill = GetBill(CreateApprovedBill());
            var payment = service.PayBill(bill, cashAccount, amount: 50);
            if (payment?.Id != null) VoidPayment(payment.Id);
            Assert.IsNotNull(payment);
        }
    }
}
