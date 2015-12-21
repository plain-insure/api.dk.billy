using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BillyService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BillyService.Tests
{
    [TestClass]
    public class ContactsTest
    {
        private Contacts service = new Contacts("API_KEY");

        [TestMethod]
        public void Get()
        {
            var create = service.Create(new Contact
            {
                organizationId = "",
                name = "",
                countryId = Countries.DK.ToString(),
                street = "",
                zipcodeText = "",
                cityText = "",
                phone = "",
                isCustomer = true,
                isSupplier = false,
                contactPersons = new List<ContactPerson>
                {
                    new ContactPerson
                    {
                        email = "",
                        name = ""
                    }
                }
            });

            // Act
            var result = service.Get(create);

            var deleteResult = service.Delete(create);

            // Assert
            Assert.AreEqual(create, result.id);
        }

        [TestMethod]
        public void List()
        {
            var result = service.List();

            // Cleanup

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Create()
        {
            // Act
            var result = service.Create(new Contact
            {
                organizationId = "",
                name = "",
                countryId = Countries.DK.ToString(),
                street = "",
                zipcodeText = "",
                cityText = "",
                phone = "",
                isCustomer = true,
                isSupplier = false,
                contactPersons = new List<ContactPerson>
                {
                    new ContactPerson
                    {
                        email = "",
                        name = ""
                    }
                }
            });

            var deleteResult = service.Delete(result);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Delete()
        {
            // Arrange
            var create = service.Create(new Contact
            {
                organizationId = "",
                name = "",
                countryId = Countries.DK.ToString(),
                street = "",
                zipcodeText = "",
                cityText = "",
                phone = "",
                isCustomer = true,
                isSupplier = false,
                contactPersons = new List<ContactPerson>
                {
                    new ContactPerson
                    {
                        email = "",
                        name = ""
                    }
                }
            });

            // Act
            var result = service.Delete(create);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
