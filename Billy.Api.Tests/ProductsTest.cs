using Billy.Api.Models;
using RestSharp;

namespace Billy.Api.Tests
{
    [TestClass]
    public class ProductsTest : TestBase<Products>
    {
        public override Products CreateService(RestClient client) => new(client);

        [TestMethod]
        public void Get()
        {
            var id = service.Create(new Product
            {
                OrganizationId = OrganizationId,
                AccountId = "0rNTvLTZS32KDV0TFTlm5w",
                Name = "Test Get Product",
                Unit = "pieces"
            });

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
            var id = service.Create(new Product
            {
                OrganizationId = OrganizationId,
                Name = "Test Create Product",
                AccountId = "0rNTvLTZS32KDV0TFTlm5w",
                Unit = "pieces"
            });

            service.Delete(id);

            Assert.IsNotNull(id);
        }

        [TestMethod]
        public void Delete()
        {
            var id = service.Create(new Product
            {
                OrganizationId = OrganizationId,
                Name = "Test Delete Product",
                AccountId = "0rNTvLTZS32KDV0TFTlm5w",
                Unit = "pieces"
            });

            var result = service.Delete(id);

            Assert.IsNotNull(result);
        }
    }
}
