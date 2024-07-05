namespace Billy.Api.Tests
{
    public abstract class TestBase
    {
        internal static string ApiKey => Environment.GetEnvironmentVariable("BILLY_TEST_APIKEY") ?? throw new ArgumentNullException("BILLY_TEST_APIKEY");
        internal static string OrganizationId => Environment.GetEnvironmentVariable("BILLY_TEST_ORG_ID") ?? throw new ArgumentNullException("BILLY_TEST_ORG_ID");
    }
}
