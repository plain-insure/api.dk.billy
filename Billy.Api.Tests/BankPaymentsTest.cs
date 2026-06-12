using Billy.Api.Models;
using RestSharp;

namespace Billy.Api.Tests
{
    [TestClass]
    public class BankPaymentsTest : TestBase<BankPayments>
    {
        public override BankPayments CreateService(RestClient client) => new(client);

        private Account cashAccount = default!;
        private string contactId = default!;
        private string? existingPaymentId;
        private string? existingNonVoidedPaymentId;

        [TestInitialize]
        public void InitializeTestData()
        {
            cashAccount = new Accounts(Client).List()
                ?.FirstOrDefault(a => a.IsPaymentEnabled)
                ?? throw new InvalidOperationException("No payment-enabled account found");

            contactId = new Contacts(Client).List()
                ?.FirstOrDefault()?.Id
                ?? throw new InvalidOperationException("No contacts found in the organisation");

            existingPaymentId = service.List(new { organizationId = OrganizationId })
                ?.FirstOrDefault()?.Id;

            existingNonVoidedPaymentId = service.List(new { organizationId = OrganizationId, isVoided = false })
                ?.FirstOrDefault()?.Id;
        }

        private BankPayment BuildPayment() => new()
        {
            OrganizationId = OrganizationId,
            ContactId = contactId,
            EntryDate = DateOnly.FromDateTime(DateTime.Today),
            CashAmount = 1.00m,
            CashSide = CashSide.Debit,
            CashAccountId = cashAccount.Id,
            SubjectCurrencyId = cashAccount.CurrencyId
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
            var payment = service.Create(BuildPayment());
            VoidPayment(payment.Id);
            Assert.IsNotNull(payment);
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        [TestMethod]
        public void Get()
        {
            var created = existingPaymentId == null;
            var id = existingPaymentId ?? service.Create(BuildPayment())?.Id;
            try
            {
                var result = service.Get(id);
                Assert.AreEqual(id, result.Id);
            }
            finally
            {
                if (created) VoidPayment(id);
            }
        }

        // ── Void (only mutable field after creation) ─────────────────────────────
        // Also exercises isVoided=false filtering via InitializeTestData lookup

        [TestMethod]
        public void Void()
        {
            var id = existingNonVoidedPaymentId ?? service.Create(BuildPayment())?.Id;

            service.Update(id, new DeltaObject<BankPayment>().Set(p => p.IsVoided, true));

            var result = service.Get(id);
            Assert.IsTrue(result?.IsVoided ?? false);
        }
    }
}
