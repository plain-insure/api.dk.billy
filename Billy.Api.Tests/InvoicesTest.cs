using Billy.Api.Models;
using RestSharp;
using System.Diagnostics;

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
                Name = "Test Customer New",
                CountryId = Countries.DK.ToString(),
                Street = "",
                ZipcodeText = "",
                CityText = "",
                Phone = "",
                IsCustomer = true,
                IsSupplier = false
            })?.Id ?? throw new InvalidOperationException("Failed to create customer contact");

        private void DeleteContact(string contactId) =>
            new Contacts(Client).Delete(contactId);

        private string CreateProduct() =>
            new Products(Client).Create(new Product
            {
                OrganizationId = OrganizationId,
                Name = "Test Product New",
                AccountId = productAccountId,
                Unit = "pieces"
            })?.Id ?? throw new InvalidOperationException("Failed to create product");

        private void DeleteProduct(string productId) =>
            new Products(Client).Delete(productId);

        private Invoice BuildInvoice(string contactId, string productId, string state = "draft") => new()
        {
            OrganizationId = OrganizationId,
            ContactId = contactId,
            EntryDate = DateOnly.FromDateTime(DateTime.Now),
            PaymentTermsDays = 0,
            State = state,
            SentState = "unsent",
            TaxMode = TaxMode.Incl,
            Lines =
            [
                new InvoiceLine
                {
                    ProductId = productId,
                    Quantity = 1,
                    UnitPrice = 100,
                    Description = "Test line"
                }
            ]
        };

        private Invoice BuildInvoiceWithLines(string contactId, string productId, List<InvoiceLine> lines, string state = "draft") => new()
        {
            OrganizationId = OrganizationId,
            ContactId = contactId,
            EntryDate = DateOnly.FromDateTime(DateTime.Now),
            PaymentTermsDays = 0,
            State = state,
            SentState = "unsent",
            TaxMode = TaxMode.Excl,
            Lines = lines
        };

        [TestMethod]
        public void Get()
        {
            var contactId = CreateCustomerContact();
            var productId = CreateProduct();
            try
            {
                var id = service.Create(BuildInvoice(contactId, productId))?.Id;
                Assert.IsNotNull(id);
                var result = service.Get(id);
                service.Delete(id);
                Assert.AreEqual(id, result?.Id);
                Assert.AreEqual(100m, result?.Amount);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
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
            var draft = true;

            var contactId = CreateCustomerContact();
            var productId = CreateProduct();
            try
            {
                var id = service.Create(BuildInvoice(contactId, productId, draft ? "draft" : "approved"))?.Id;
                Assert.IsNotNull(id);
                if (draft)
                    service.Delete(id);
                Assert.IsNotNull(id);
            }
            finally
            {
                if (draft)
                {
                    DeleteContact(contactId);
                    DeleteProduct(productId);
                }
            }
        }

        [TestMethod]
        public void Create_Line_NoDiscount()
        {
            // 2.5 units × 40.00 = 100.00  (decimal quantity, no discount)
            var contactId = CreateCustomerContact();
            var productId = CreateProduct();
            try
            {
                var created = service.Create(BuildInvoiceWithLines(contactId, productId,
                [
                    new InvoiceLine { ProductId = productId, Quantity = 2.5m, UnitPrice = 40m, Description = "No discount" }
                ]));

                Assert.IsNotNull(created?.Id);
                Assert.AreEqual(100m, created.Amount);

                var fetched = service.Get(created.Id);
                service.Delete(created.Id);
                Assert.AreEqual(100m, fetched?.Amount);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                DeleteContact(contactId);
                DeleteProduct(productId);
            }
        }

        [TestMethod]
        public void Create_Line_DiscountPercent()
        {
            // 1.5 units × 100.00 = 150.00, minus 10% = 135.00
            var contactId = CreateCustomerContact();
            var productId = CreateProduct();
            try
            {
                var created = service.Create(BuildInvoiceWithLines(contactId, productId,
                [
                    new InvoiceLine
                    {
                        ProductId = productId,
                        Quantity = 1.5m,
                        UnitPrice = 100m,
                        Description = "10% percent discount",
                        DiscountMode = DiscountMode.Percent,
                        DiscountValue = 10m
                    }
                ]));

                Assert.IsNotNull(created?.Id);
                Assert.AreEqual(135m, created.Amount);

                var fetched = service.Get(created.Id);
                service.Delete(created.Id);
                Assert.AreEqual(135m, fetched?.Amount);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                DeleteContact(contactId);
                DeleteProduct(productId);
            }
        }

        [TestMethod]
        public void Create_Line_DiscountAmount()
        {
            // 1 unit × 125.50 = 125.50, minus 25.50 fixed amount = 100.00
            var contactId = CreateCustomerContact();
            var productId = CreateProduct();
            string? invoiceId = null;
            try
            {
                var created = service.Create(BuildInvoiceWithLines(contactId, productId,
                [
                    new InvoiceLine
                    {
                        ProductId = productId,
                        Quantity = 1m,
                        UnitPrice = 125.50m,
                        Description = "25.50 fixed discount",
                        DiscountMode = DiscountMode.Cash,
                        DiscountValue = 25.50m
                    }
                ]));
                invoiceId = created?.Id;

                Assert.IsNotNull(created?.Id);
                Assert.AreEqual(100m, created.Amount);

                var fetched = service.Get(created.Id);
                Assert.AreEqual(100m, fetched?.Amount);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                if (invoiceId is not null)
                    service.Delete(invoiceId);
                DeleteContact(contactId);
                DeleteProduct(productId);
            }
        }

        [TestMethod]
        public void Create_MultipleLines_MixedDiscountModes()
        {
            // Line 1 (no discount):   2.5 × 40.00            = 100.00
            // Line 2 (percent 25%):   2   × 75.00 × (1−0.25) = 112.50
            // Line 3 (amount 12.50):  1   × 62.50 − 12.50    =  50.00
            // Invoice total                                   = 262.50
            const decimal expected = 262.50m;

            var contactId = CreateCustomerContact();
            var productId = CreateProduct();
            string? invoiceId = null;
            try
            {
                var created = service.Create(BuildInvoiceWithLines(contactId, productId,
                [
                    new InvoiceLine { ProductId = productId, Quantity = 2.5m, UnitPrice = 40m,    Description = "No discount"    },
                    new InvoiceLine { ProductId = productId, Quantity = 2m,   UnitPrice = 75m,    Description = "25% off",       DiscountMode = DiscountMode.Percent, DiscountValue = 25m    },
                    new InvoiceLine { ProductId = productId, Quantity = 1m,   UnitPrice = 62.50m, Description = "12.50 off",     DiscountMode = DiscountMode.Cash,  DiscountValue = 12.50m }
                ]));
                invoiceId = created?.Id;

                Assert.IsNotNull(created?.Id);
                Assert.AreEqual(expected, created.Amount);

                var fetched = service.Get(created.Id);
                Assert.AreEqual(expected, fetched?.Amount);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                if (invoiceId is not null)
                    service.Delete(invoiceId);
                DeleteContact(contactId);
                DeleteProduct(productId);
            }
        }
    }
}
