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
        public void List()
        {
            var result = service.List();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Create()
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

        [TestMethod]
        public void Delete()
        {
            var contactId = CreateSupplierContact();
            try
            {
                var id = service.Create(BuildDraftBill(contactId));
                var result = service.Delete(id);
                Assert.IsNotNull(result);
            }
            finally
            {
                DeleteContact(contactId);
            }
        }
    }
}
