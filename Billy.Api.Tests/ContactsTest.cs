using Billy.Api.Models;

namespace Billy.Api.Tests
{
    [TestClass]
    public class ContactsTest : TestBase
    {
        private readonly Contacts service = new(ApiKey);

        //TODO: Fix this test
        public void Get()
        {
            var create = service.Create(new Contact
            {
                OrganizationId = OrganizationId,
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

        //TODO: Fix this test
        public void Create()
        {
            // Act
            var result = service.Create(new Contact
            {
                OrganizationId = OrganizationId,
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


        //TODO: Fix this test
        public void Delete()
        {
            // Arrange
            var create = service.Create(new Contact
            {
                OrganizationId = OrganizationId,
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
            Assert.IsNotNull(result);
        }
    }
}
