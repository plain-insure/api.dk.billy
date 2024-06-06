using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Billy.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billy.Api.Tests
{
    [TestClass]
    public class ContactsTest
    {
        private Contacts service = new Contacts(Environment.GetEnvironmentVariable("BILLY_TEST_APIKEY"));

        [TestMethod]
        public void Get()
        {
            var create = service.Create(new Contact
            {
                OrganizationId = "",
                Name = "",
                CountryId = Countries.DK.ToString(),
                Street = "",
                ZipcodeText = "",
                CityText = "",
                Phone = "",
                IsCustomer = true,
                IsSupplier = false,
                ContactPersons = new List<ContactPerson>
                {
                    new ContactPerson
                    {
                        Email = "",
                        Name = ""
                    }
                }
            });

            // Act
            var result = service.Get(create);

            var deleteResult = service.Delete(create);

            // Assert
            Assert.AreEqual(create, result.Id);
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
                OrganizationId = "",
                Name = "",
                CountryId = Countries.DK.ToString(),
                Street = "",
                ZipcodeText = "",
                CityText = "",
                Phone = "",
                IsCustomer = true,
                IsSupplier = false,
                ContactPersons = new List<ContactPerson>
                {
                    new ContactPerson
                    {
                        Email = "",
                        Name = ""
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
                OrganizationId = "",
                Name = "",
                CountryId = Countries.DK.ToString(),
                Street = "",
                ZipcodeText = "",
                CityText = "",
                Phone = "",
                IsCustomer = true,
                IsSupplier = false,
                ContactPersons = new List<ContactPerson>
                {
                    new ContactPerson
                    {
                        Email = "",
                        Name = ""
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
