using Billy.Api.Utils;

namespace Billy.Api.Tests
{
    public abstract class TestBase<T>
    {
        internal static string ApiKey => Environment.GetEnvironmentVariable("BILLY_TEST_APIKEY") ?? throw new ArgumentNullException("BILLY_TEST_APIKEY");
        internal static string OrganizationId => Environment.GetEnvironmentVariable("BILLY_TEST_ORG_ID") ?? throw new ArgumentNullException("BILLY_TEST_ORG_ID");



        public TestContext TestContext { get; set; } = default!;
        private readonly BillyDebugLog billyDebugLog = new();

        public abstract T CreateService(RestSharp.RestClient client);

#pragma warning disable CS8618 
        internal T service;
        protected RestSharp.RestClient Client { get; private set; }
#pragma warning restore CS8618 

        [TestInitialize]
        public void Initialize()
        {
            var httpClient = new HttpClient();
            Client = ClientExtensions.CreateBillyClient(httpClient, ApiKey, billyDebugLog);
            service = CreateService(Client);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (TestContext.CurrentTestOutcome != UnitTestOutcome.Passed)
            {
                TestContext.WriteLine(billyDebugLog.ToString());
                billyDebugLog.Clear();
            }
        }

    }
}
