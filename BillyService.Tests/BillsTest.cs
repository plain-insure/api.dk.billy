using BillyService.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BillyService.Tests
{
    [TestClass]
    public class BillServiceTest
    {
        private Bills service = new Bills("");

        [TestMethod]
        public void Get()
        {
            // Arrange
            var id = service.Create(new Bill
            {
                organizationId = "",
                contactId = "",
                entryDate = DateTime.Now.ToString("yyyy-MM-dd"),
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
            // Act
            var result = service.Create(new Bill
            {
                organizationId = "",
                contactId = "",
                entryDate = DateTime.Now.ToString("yyyy-MM-dd"),
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
    }
}
