namespace Billy.Api.Tests
{
    [TestClass]
    public class OrganizationTest : TestBase
    {

        private readonly Organizations service = new(ApiKey);

        [TestMethod]
        public void Get()
        {
            // Act
            var result = service.Get(OrganizationId);

            // Assert
            Assert.IsNotNull(result);

            Assert.AreEqual(result.Id, OrganizationId);
        }

        //[TestMethod]
        //This will only work with Basic Auth => on login endpoint, NOT API Key
        public void List()
        {
            // Act
            var result = service.List();

            // Assert
            Assert.IsNotNull(result);
        }


    }
}
