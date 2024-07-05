using Billy.Api.Models;

namespace Billy.Api.Tests
{
    [TestClass]
    public class BillServiceTest : TestBase
    {
        private readonly Bills service = new(ApiKey);

        //TODO: Fix this test
        public void Get()
        {
            // Arrange
            var id = service.Create(new Bill
            {
                OrganizationId = OrganizationId,
                ContactId = "",
                EntryDate = DateTime.Now,
                State = BillStates.draft,
                TaxMode = "incl",
                SuppliersInvoiceNo = "",
                Lines = new List<BillLine>
                {
                    new BillLine
                    {
                        AccountId = "",
                        Amount = 0,
                        Description = "",
                        TaxRateId = ""
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

        /*
        [TestMethod]
        public void Create()
        {
            // Act
            var result = service.Create(new Bill
            {
                organizationId = "",
                contactId = "",
                entryDate = DateTime.Now,
                state = "approved",
                taxMode = "incl",
                suppliersInvoiceNo = "",
                lines = new List<BillLine>
                {
                    new BillLine
                    {
                        accountId = "",
                        amount = 0,
                        description = "",
                        taxRateId = ""
                    }
                }
            });

            // Assert
            Assert.IsNotNull(result);
        }
        */
    }
}
