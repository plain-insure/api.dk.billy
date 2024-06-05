﻿

using Billy.Models;
using BillyService.Models;

namespace BillyService.Tests
{
    [TestClass]
    public class InvoicesTest
    {
        private Invoices service = new Invoices("");

        [TestMethod]
        public void Get()
        {
            // Arrange
            var id = service.Create(new Invoice
            {
                organizationId = "",
                contactId = "",
                entryDate = DateTime.Now,
                paymentTermsDays = 0,
                state = "approved",
                sentState = "unsent",
                taxMode = "incl",
                invoiceNo = "",
                lines = new List<InvoiceLine>
                {
                    new InvoiceLine
                    {
                        unitPrice = 0,
                        productId = "",
                        description = ""
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

        [TestMethod]
        public void Create()
        {
            var result = service.Create(new Invoice
            {
                organizationId = "",
                contactId = "",
                entryDate = DateTime.Now,
                paymentTermsDays = 0,
                state = "approved",
                sentState = "unsent",
                taxMode = "incl",
                invoiceNo = "",
                lines = new List<InvoiceLine>
                {
                    new InvoiceLine
                    {
                        unitPrice = 0,
                        productId = "",
                        description = ""
                    }
                }
            });

            Assert.IsNotNull(result);
        }
    }
}
