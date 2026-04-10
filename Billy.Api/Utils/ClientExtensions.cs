using RestSharp;

namespace Billy.Api.Utils
{
    public static class ClientExtensions
    {
        private static readonly Uri BillyApiBaseUri = new("https://api.billysbilling.com/v2/");

        public static void AddBillyAuthentication(this RestSharp.RestClient client, string key)
        {
            client.AddDefaultHeader("X-Access-Token", key);
        }

        public static RestClient CreateBillyClient(HttpClient httpClient, string key)
            => CreateBillyClient(httpClient, key, (Action<string>?)null);

        public static RestClient CreateBillyClient(HttpClient httpClient, string key, BillyDebugLog debugLog)
            => CreateBillyClient(httpClient, key, debugLog.Write);

        public static RestClient CreateBillyClient(HttpClient httpClient, string key, Action<string>? debugLog)
        {
            httpClient.BaseAddress = BillyApiBaseUri;
            var client = debugLog is null
                ? new RestClient(httpClient)
                : new RestClient(
                    httpClient,
                    false,
                    options =>
                    {
                        options.BaseUrl = BillyApiBaseUri;
                        options.Interceptors = [new BillyHttpDebugInterceptor(debugLog)];
                    },
                    null);
            client.AddBillyAuthentication(key);
            return client;
        }

        public static RestClient CreateBillyClient(string key)
            => CreateBillyClient(key, (Action<string>?)null);

        public static RestClient CreateBillyClient(string key, BillyDebugLog debugLog)
            => CreateBillyClient(key, debugLog.Write);

        public static RestClient CreateBillyClient(string key, Action<string>? debugLog)
        {
            var client = debugLog is null
                ? new RestClient(BillyApiBaseUri)
                : new RestClient(
                    BillyApiBaseUri,
                    options => options.Interceptors = [new BillyHttpDebugInterceptor(debugLog)]);
            client.AddBillyAuthentication(key);
            return client;
        }
    }
}
