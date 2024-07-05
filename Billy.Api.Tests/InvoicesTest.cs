

using Billy.Api.Models;

namespace Billy.Api.Tests
{
    [TestClass]
    public class InvoicesTest : TestBase
    {
        private readonly Invoices service = new(ApiKey);

        //TODO: Fix this test
        public void Get()
        {
            // Arrange
            var id = service.Create(new Invoice
            {
                OrganizationId = OrganizationId,
                ContactId = "",
                EntryDate = DateTime.Now,
                PaymentTermsDays = 0,
                State = "approved",
                SentState = "unsent",
                TaxMode = "incl",
                InvoiceNo = "",
                Lines = new List<InvoiceLine>
                {
                    new InvoiceLine
                    {
                        UnitPrice = 0,
                        ProductId = "",
                        Description = ""
                    }
                }
            });

            // Act
            var result = service.Get(id);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void List()
        {
            // Act
            var result = service.List();

            // Assert
            Assert.IsNotNull(result);
        }

        //TODO: Fix this test
        public void Create()
        {
            var result = service.Create(new Invoice
            {
                OrganizationId = OrganizationId,
                ContactId = "",
                EntryDate = DateTime.Now,
                PaymentTermsDays = 0,
                State = "approved",
                SentState = "unsent",
                TaxMode = "incl",
                InvoiceNo = "",
                Lines = new List<InvoiceLine>
                {
                    new InvoiceLine
                    {
                        UnitPrice = 0,
                        ProductId = "",
                        Description = ""
                    }
                }
            });

            Assert.IsNotNull(result);
        }
    }
}
