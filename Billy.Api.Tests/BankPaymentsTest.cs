using Billy.Api.Models;
using RestSharp;

namespace Billy.Api.Tests
{
    [TestClass]
    public class BankPaymentsTest : TestBase<BankPayments>
    {
        public override BankPayments CreateService(RestClient client) => new(client);

        private string cashAccountId = default!;
        private string contactId = default!;

        [TestInitialize]
        public void InitializeTestData()
        {
            cashAccountId = new Accounts(Client).List()
                ?.FirstOrDefault(a => a.IsPaymentEnabled)?.Id
                ?? throw new InvalidOperationException("No payment-enabled account found");

            contactId = new Contacts(Client).Create(new Contact
            {
                Type = "company",
                OrganizationId = OrganizationId,
                Name = "Test Payment Contact",
                CountryId = Countries.DK.ToString(),
                Street = "",
                ZipcodeText = "",
                CityText = "",
                Phone = "",
                IsCustomer = true,
                IsSupplier = false
            }) ?? throw new InvalidOperationException("Failed to create contact");
        }

        [TestCleanup]
        public void CleanupTestData()
        {
            if (contactId != null)
                new Contacts(Client).Delete(contactId);
        }

        private BankPayment BuildPayment() => new()
        {
            OrganizationId = OrganizationId,
            ContactId = contactId,
            EntryDate = DateTime.Today,
            CashAmount = 1.00,
            CashSide = CashSide.debit,
            CashAccountId = cashAccountId
        };

        private void VoidPayment(string id) =>
            service.Update(id, new DeltaObject<BankPayment>().Set(p => p.IsVoided, true));

        // ── List ────────────────────────────────────────────────────────────────

        [TestMethod]
        public void List()
        {
            var result = service.List(new { organizationId = OrganizationId });
            Assert.IsNotNull(result);
        }

        // ── Create ───────────────────────────────────────────────────────────────

        [TestMethod]
        public void Create()
        {
            var id = service.Create(BuildPayment());
            VoidPayment(id);
            Assert.IsNotNull(id);
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        [TestMethod]
        public void Get()
        {
            var id = service.Create(BuildPayment());
            try
            {
                var result = service.Get(id);
                Assert.AreEqual(id, result.Id);
            }
            finally
            {
                VoidPayment(id);
            }
        }

        // ── Void (only mutable field after creation) ─────────────────────────────

        [TestMethod]
        public void Void()
        {
            var id = service.Create(BuildPayment());

            service.Update(id, new DeltaObject<BankPayment>().Set(p => p.IsVoided, true));

            var result = service.Get(id);
            Assert.IsTrue(result?.IsVoided ?? false);
        }
    }
}
