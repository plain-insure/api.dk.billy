using Billy.Api.Models;
using RestSharp;

namespace Billy.Api.Tests
{
    [TestClass]
    public class DaybooksTest : TestBase<Daybooks>
    {
        public override Daybooks CreateService(RestClient client) => new(client);

        private Daybook BuildDaybook() => new()
        {
            OrganizationId = OrganizationId,
            Name = "Test Daybook",
            IsTransactionSummaryEnabled = false
        };

        [TestMethod]
        public void List()
        {
            var result = service.List(new { organizationId = OrganizationId });
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Get()
        {
            var id = service.Create(BuildDaybook());
            try
            {
                var result = service.Get(id);
                Assert.AreEqual(id, result.Id);
            }
            finally
            {
                service.Delete(id);
            }
        }

        [TestMethod]
        public void Create()
        {
            var id = service.Create(BuildDaybook());
            service.Delete(id);
            Assert.IsNotNull(id);
        }

        [TestMethod]
        public void Delete()
        {
            var id = service.Create(BuildDaybook());
            var result = service.Delete(id);
            Assert.IsNotNull(result);
        }
    }
}
