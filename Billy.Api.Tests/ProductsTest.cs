using Billy.Api.Models;
using RestSharp;

namespace Billy.Api.Tests
{
    [TestClass]
    public class ProductsTest : TestBase<Products>
    {
        public override Products CreateService(RestClient client) => new(client);

        private string productAccountId = default!;

        [TestInitialize]
        public void InitializeTestData()
        {
            productAccountId = new Accounts(Client).List()
                ?.FirstOrDefault(a => a.NatureId == "revenue")?.Id
                ?? throw new InvalidOperationException("No revenue account found in the organisation");
        }

        private Product BuildTestProduct(string name) => new()
        {
            OrganizationId = OrganizationId,
            Name = name,
            AccountId = productAccountId,
            Unit = "pieces"
        };

        [TestMethod]
        public void Get()
        {
            var id = service.Create(BuildTestProduct("Test Get Product"));

            var result = service.Get(id);

            service.Delete(id);

            Assert.AreEqual(id, result.Id);
        }

        [TestMethod]
        public void List()
        {
            var result = service.List();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Create()
        {
            var id = service.Create(BuildTestProduct("Test Create Product"));

            service.Delete(id);

            Assert.IsNotNull(id);
        }

        [TestMethod]
        public void Delete()
        {
            var id = service.Create(BuildTestProduct("Test Delete Product"));

            var result = service.Delete(id);

            Assert.IsNotNull(result);
        }
    }
}
