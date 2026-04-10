using Billy.Api.Models;
using RestSharp;

namespace Billy.Api.Tests
{
    [TestClass]
    public class InvoicesTest : TestBase<Invoices>
    {
        public override Invoices CreateService(RestClient client) => new(client);

        private string productAccountId = default!;

        [TestInitialize]
        public void InitializeTestData()
        {
            productAccountId = new Accounts(Client).List()
                ?.FirstOrDefault(a => a.NatureId == "revenue")?.Id
                ?? throw new InvalidOperationException("No revenue account found in the organisation");
        }

        private string CreateCustomerContact() =>
            new Contacts(Client).Create(new Contact
            {
                Type = "company",
                OrganizationId = OrganizationId,
                Name = "Test Customer",
                CountryId = Countries.DK.ToString(),
                Street = "",
                ZipcodeText = "",
                CityText = "",
                Phone = "",
                IsCustomer = true,
                IsSupplier = false
            }) ?? throw new InvalidOperationException("Failed to create customer contact");

        private void DeleteContact(string contactId) =>
            new Contacts(Client).Delete(contactId);

        private string CreateProduct() =>
            new Products(Client).Create(new Product
            {
                OrganizationId = OrganizationId,
                Name = "Test Product",
                AccountId = productAccountId,
                Unit = "pieces"
            }) ?? throw new InvalidOperationException("Failed to create product");

        private void DeleteProduct(string productId) =>
            new Products(Client).Delete(productId);

        private Invoice BuildDraftInvoice(string contactId, string productId) => new()
        {
            OrganizationId = OrganizationId,
            ContactId = contactId,
            EntryDate = DateTime.Now,
            PaymentTermsDays = 0,
            State = "draft",
            SentState = "unsent",
            TaxMode = "incl",
            Lines =
            [
                new InvoiceLine
                {
                    ProductId = productId,
                    UnitPrice = 100,
                    Description = "Test line"
                }
            ]
        };

        [TestMethod]
        public void Get()
        {
            var contactId = CreateCustomerContact();
            var productId = CreateProduct();
            try
            {
                var id = service.Create(BuildDraftInvoice(contactId, productId));
                var result = service.Get(id);
                service.Delete(id);
                Assert.AreEqual(id, result.Id);
            }
            finally
            {
                DeleteContact(contactId);
                DeleteProduct(productId);
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
            var contactId = CreateCustomerContact();
            var productId = CreateProduct();
            try
            {
                var id = service.Create(BuildDraftInvoice(contactId, productId));
                service.Delete(id);
                Assert.IsNotNull(id);
            }
            finally
            {
                DeleteContact(contactId);
                DeleteProduct(productId);
            }
        }
    }
}
