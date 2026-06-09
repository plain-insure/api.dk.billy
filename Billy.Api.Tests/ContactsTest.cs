using Billy.Api.Models;
using Billy.Api.Utils;
using RestSharp;
using System.Diagnostics.CodeAnalysis;

namespace Billy.Api.Tests
{
    [TestClass]
    public class ContactsTest : TestBase<Contacts>
    {

        public override Contacts CreateService(RestClient client) => new(client);

        [TestMethod]
        public void Get()
        {


            var create = service.Create(new Contact
            {
                Type = "company",
                OrganizationId = OrganizationId,
                Name = "Test Get Company",
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
                        Email = "test@example.com",
                        Name = "Test Person"
                    }
                }
            });

            Assert.IsNotNull(create?.Id);

            // Act
            var result = service.Get(create.Id);

            var deleteResult = service.Delete(create.Id);

            // Assert
            Assert.AreEqual(create.Id, result?.Id);
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
                Type = "company",
                OrganizationId = OrganizationId,
                Name = "Test Create Company",
                CountryId = Countries.DK.ToString(),
                Street = "",
                ZipcodeText = "",
                CityText = "",
                Phone = "",
                IsCustomer = true,
                IsSupplier = false
            });
            Assert.IsNotNull(result?.Id);

            var deleteResult = service.Delete(result.Id);

            // Assert
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void Delete()
        {
            // Arrange
            var create = service.Create(new Contact
            {
                Type = "company",
                OrganizationId = OrganizationId,
                Name = "Test Delete Company",
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

            Assert.IsNotNull(create?.Id);

            // Act
            var result = service.Delete(create.Id);

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
