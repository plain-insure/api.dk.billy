using RestSharp;

namespace Billy.Api.Tests
{
    [TestClass]
    public class TaxRatesTest : TestBase<TaxRates>
    {
        public override TaxRates CreateService(RestClient client) => new(client);

        [TestMethod]
        public void List()
        {
            var result = service.List();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public void Get()
        {
            var all = service.List();
            var first = all?.FirstOrDefault();

            Assert.IsNotNull(first, "No tax rates found in the organisation");

            var result = service.Get(first.Id);

            Assert.AreEqual(first.Id, result.Id);
        }
    }
}
