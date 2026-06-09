using Billy.Api.Models;
using RestSharp;

namespace Billy.Api.Tests
{
    [TestClass]
    public class BillServiceTest : TestBase<Bills>
    {
        public override Bills CreateService(RestClient client) => new(client);

        private string accountId = default!;

        [TestInitialize]
        public void InitializeTestData()
        {
            accountId = new Accounts(Client).List()
                ?.FirstOrDefault(a => a.AccountNo >= 1200 && a.AccountNo <= 1299)?.Id
                ?? throw new InvalidOperationException("No account found with account number between 1200 and 1299");
        }

        private string CreateSupplierContact() =>
            new Contacts(Client).Create(new Contact
            {
                Type = "company",
                OrganizationId = OrganizationId,
                Name = "Test Supplier",
                CountryId = Countries.DK.ToString(),
                Street = "",
                ZipcodeText = "",
                CityText = "",
                Phone = "",
                IsCustomer = false,
                IsSupplier = true
            }) ?? throw new InvalidOperationException("Failed to create supplier contact");

        private void DeleteContact(string contactId) =>
            new Contacts(Client).Delete(contactId);

        private Bill BuildDraftBill(string contactId) => new()
        {
            OrganizationId = OrganizationId,
            ContactId = contactId,
            EntryDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(14),
            State = BillStates.draft,
            TaxMode = "incl",
            Lines =
            [
                new BillLine
                {
                    AccountId = accountId,
                    Amount = 100,
                    Description = "Test line"
                }
            ]
        };

        // ── List ────────────────────────────────────────────────────────────────

        [TestMethod]
        public void List()
        {
            var result = service.List();

            Assert.IsNotNull(result);
        }

        // ── Create / Delete ──────────────────────────────────────────────────────

        [TestMethod]
        public void CreateDelete()
        {
            var contactId = CreateSupplierContact();
            try
            {
                var id = service.Create(BuildDraftBill(contactId));
                service.Delete(id);
                Assert.IsNotNull(id);
            }
            finally
            {
                DeleteContact(contactId);
            }
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        [TestMethod]
        public void Get()
        {
            var contactId = CreateSupplierContact();
            try
            {
                var id = service.Create(BuildDraftBill(contactId));
                var result = service.Get(id);
                service.Delete(id);
                Assert.AreEqual(id, result.Id);
            }
            finally
            {
                DeleteContact(contactId);
            }
        }

        [TestMethod]
        public void GetWithoutSideload_LinesIsNull()
        {
            var contactId = CreateSupplierContact();
            try
            {
                var id = service.Create(BuildDraftBill(contactId));
                var result = service.Get(id);
                service.Delete(id);
                Assert.IsNull(result?.Lines);
            }
            finally
            {
                DeleteContact(contactId);
            }
        }

        [TestMethod]
        public void GetWithSideload_LinesIsPopulated()
        {
            var contactId = CreateSupplierContact();
            try
            {
                service.SideloadLines();
                var id = service.Create(BuildDraftBill(contactId));
                var result = service.Get(id);
                service.Delete(id);
                Assert.IsNotNull(result?.Lines);
                Assert.AreEqual(1, result.Lines.Count);
            }
            finally
            {
                DeleteContact(contactId);
            }
        }

        // ── Update via full object ────────────────────────────────────────────────

        [TestMethod]
        public void UpdateOriginal_StringField()
        {
            var contactId = CreateSupplierContact();
            try
            {
                var id = service.Create(BuildDraftBill(contactId));
                var result = service.Get(id);
                result.SuppliersInvoiceNo = "INV-001";
                service.Update(result);

                var updated = service.Get(id);
                service.Delete(id);

                Assert.IsNotNull(updated);
                Assert.AreEqual("INV-001", updated.SuppliersInvoiceNo);
            }
            finally
            {
                DeleteContact(contactId);
            }
        }

        [TestMethod]
        public void UpdateOriginal_DateField()
        {
            // DueDate uses [JsonConverter(typeof(BillyDateConverter))] — date-only format "yyyy-MM-dd"
            var contactId = CreateSupplierContact();
            try
            {
                var dueDate = DateTime.Now.AddDays(30);
                var id = service.Create(BuildDraftBill(contactId));
                var result = service.Get(id);
                result.DueDate = dueDate;
                service.Update(result);

                var updated = service.Get(id);
                service.Delete(id);

                Assert.IsNotNull(updated);
                Assert.AreEqual(dueDate.Date, updated.DueDate.Date);
            }
            finally
            {
                DeleteContact(contactId);
            }
        }

        [TestMethod]
        public void UpdateOriginal_MultipleFields()
        {
            var contactId = CreateSupplierContact();
            try
            {
                var dueDate = DateTime.Now.AddDays(45);
                var id = service.Create(BuildDraftBill(contactId));
                var result = service.Get(id);
                result.SuppliersInvoiceNo = "INV-MULTI";
                result.DueDate = dueDate;
                service.Update(result);

                var updated = service.Get(id);
                service.Delete(id);

                Assert.IsNotNull(updated);
                Assert.AreEqual("INV-MULTI", updated.SuppliersInvoiceNo);
                Assert.AreEqual(dueDate.Date, updated.DueDate.Date);
            }
            finally
            {
                DeleteContact(contactId);
            }
        }

        [TestMethod]
        public void UpdateOriginal_WithSideload()
        {
            var contactId = CreateSupplierContact();
            try
            {
                service.SideloadLines();
                var id = service.Create(BuildDraftBill(contactId));
                var result = service.Get(id);
                result.SuppliersInvoiceNo = "INV-SIDELOAD";
                service.Update(result);

                var updated = service.Get(id);
                service.Delete(id);

                Assert.IsNotNull(updated);
                Assert.AreEqual("INV-SIDELOAD", updated.SuppliersInvoiceNo);
                Assert.IsNotNull(updated.Lines);
                Assert.AreEqual(1, updated.Lines.Count);
            }
            finally
            {
                DeleteContact(contactId);
            }
        }

        // ── Update via DeltaObject ────────────────────────────────────────────────

        [TestMethod]
        public void UpdateDelta_StringField()
        {
            var contactId = CreateSupplierContact();
            try
            {
                var id = service.Create(BuildDraftBill(contactId));

                service.Update(id, new DeltaObject<Bill>()
                    .Set(b => b.SuppliersInvoiceNo, "DELTA-001"));

                var updated = service.Get(id);
                service.Delete(id);

                Assert.IsNotNull(updated);
                Assert.AreEqual("DELTA-001", updated.SuppliersInvoiceNo);
            }
            finally
            {
                DeleteContact(contactId);
            }
        }

        [TestMethod]
        public void UpdateDelta_DateField()
        {
            // Validates that DeltaObject respects [JsonConverter(typeof(BillyDateConverter))]
            // and serializes DueDate as "yyyy-MM-dd" rather than full ISO 8601
            var contactId = CreateSupplierContact();
            try
            {
                var dueDate = DateTime.Now.AddDays(60);
                var id = service.Create(BuildDraftBill(contactId));

                service.Update(id, new DeltaObject<Bill>()
                    .Set(b => b.DueDate, dueDate));

                var updated = service.Get(id);
                service.Delete(id);

                Assert.IsNotNull(updated);
                Assert.AreEqual(dueDate.Date, updated.DueDate.Date);
            }
            finally
            {
                DeleteContact(contactId);
            }
        }

        [TestMethod]
        public void UpdateDelta_MultipleFields()
        {
            var contactId = CreateSupplierContact();
            try
            {
                var dueDate = DateTime.Now.AddDays(90);
                var id = service.Create(BuildDraftBill(contactId));

                service.Update(id, new DeltaObject<Bill>()
                    .Set(b => b.SuppliersInvoiceNo, "DELTA-MULTI")
                    .Set(b => b.DueDate, dueDate));

                var updated = service.Get(id);
                service.Delete(id);

                Assert.IsNotNull(updated);
                Assert.AreEqual("DELTA-MULTI", updated.SuppliersInvoiceNo);
                Assert.AreEqual(dueDate.Date, updated.DueDate.Date);
            }
            finally
            {
                DeleteContact(contactId);
            }
        }

        [TestMethod]
        public void UpdateDelta_WithSideload()
        {
            var contactId = CreateSupplierContact();
            try
            {
                service.SideloadLines();
                var dueDate = DateTime.Now.AddDays(30);
                var id = service.Create(BuildDraftBill(contactId));

                service.Update(id, new DeltaObject<Bill>()
                    .Set(b => b.SuppliersInvoiceNo, "DELTA-SIDE")
                    .Set(b => b.DueDate, dueDate));

                var updated = service.Get(id);
                service.Delete(id);

                Assert.IsNotNull(updated);
                Assert.AreEqual("DELTA-SIDE", updated.SuppliersInvoiceNo);
                Assert.AreEqual(dueDate.Date, updated.DueDate.Date);
                Assert.IsNotNull(updated.Lines);
                Assert.AreEqual(1, updated.Lines.Count);
            }
            finally
            {
                DeleteContact(contactId);
            }
        }
    }
}
