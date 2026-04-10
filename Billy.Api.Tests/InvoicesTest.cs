

using Billy.Api.Models;
using RestSharp;

namespace Billy.Api.Tests
{
    [TestClass]
    public class InvoicesTest : TestBase<Invoices>
    {

        public override Invoices CreateService(RestClient client) => new(client);


        [TestMethod]
        public void Get()
        {
            // Arrange

            var id = service.Create(new Invoice
            {
                OrganizationId = OrganizationId,
                ContactId = "KB09VU96TeO84hItK2B97w",
                EntryDate = DateTime.Now,
                PaymentTermsDays = 0,
                State = "draft",
                SentState = "unsent",
                TaxMode = "incl",
                Lines = new List<InvoiceLine>
                {
                    new InvoiceLine
                    {
                        UnitPrice = 200,
                        ProductId = "ZmWlw8FzRI6mMOkdeWCcXg",
                        Description = "Test line"
                    }
                }
            });

            // Act
            var result = service.Get(id);

            // Assert
            Assert.IsNotNull(result);

            service.Delete(id); // we can delete draft invoices, so we clean up after ourselves

        }

        [TestMethod]
        public void List()
        {
            // Act
            var result = service.List();

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Create()
        {

            var result = service.Create(new Invoice
            {
                OrganizationId = OrganizationId,
                ContactId = "KB09VU96TeO84hItK2B97w",
                EntryDate = DateTime.Now,
                PaymentTermsDays = 0,
                State = "draft",
                SentState = "unsent",
                TaxMode = "incl",
                Lines = new List<InvoiceLine>
                {
                    new InvoiceLine
                    {
                        UnitPrice = 100,
                        ProductId = "ZmWlw8FzRI6mMOkdeWCcXg",
                        Description = "Test line"
                    }
                }
            });

            Assert.IsNotNull(result);

            service.Delete(result); // we can delete draft invoices, so we clean up after ourselves

        }
    }
}
