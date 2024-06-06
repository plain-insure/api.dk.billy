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
        private Bills service = new Bills(Environment.GetEnvironmentVariable("BILLY_TEST_APIKEY"));

        [TestMethod]
        public void Get()
        {
            // Arrange
            var id = service.Create(new Bill
            {
                OrganizationId = "",
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
