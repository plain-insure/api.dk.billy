using RestSharp;
using RestSharp.Serializers.Json;

namespace Billy.Api.Utils
{
    /// <summary>
    /// Factory helpers for creating a <see cref="RestClient"/> pre-configured for the Billy API.
    /// </summary>
    public static class ClientExtensions
    {
        private static readonly Uri BillyApiBaseUri = new("https://api.billysbilling.com/v2/");

        /// <summary>
        /// Adds the Billy API access token to <paramref name="client"/> as the
        /// <c>X-Access-Token</c> header required by all authenticated endpoints.
        /// </summary>
        /// <param name="client">The REST client to configure.</param>
        /// <param name="key">Billy API access token from Settings → Access tokens in mit.billy.dk.</param>
        public static void AddBillyAuthentication(this RestSharp.RestClient client, string key)
        {
            client.AddDefaultHeader("X-Access-Token", key);
        }

        /// <summary>
        /// Creates a Billy API client wrapping an existing <see cref="HttpClient"/>.
        /// Use this overload when you manage the <see cref="HttpClient"/> lifetime yourself (e.g. via DI).
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to wrap. Its <c>BaseAddress</c> will be set.</param>
        /// <param name="key">Billy API access token.</param>
        public static RestClient CreateBillyClient(HttpClient httpClient, string key)
            => CreateBillyClient(httpClient, key, (Action<string>?)null);

        /// <summary>
        /// Creates a Billy API client wrapping an existing <see cref="HttpClient"/> with debug logging.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to wrap. Its <c>BaseAddress</c> will be set.</param>
        /// <param name="key">Billy API access token.</param>
        /// <param name="debugLog">Log sink that receives raw request/response text for diagnostics.</param>
        public static RestClient CreateBillyClient(HttpClient httpClient, string key, BillyDebugLog debugLog)
            => CreateBillyClient(httpClient, key, debugLog.Write);

        /// <summary>
        /// Creates a Billy API client wrapping an existing <see cref="HttpClient"/> with a custom log callback.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to wrap. Its <c>BaseAddress</c> will be set.</param>
        /// <param name="key">Billy API access token.</param>
        /// <param name="debugLog">Callback that receives raw request/response text, or <c>null</c> to disable logging.</param>
        public static RestClient CreateBillyClient(HttpClient httpClient, string key, Action<string>? debugLog)
        {
            httpClient.BaseAddress = BillyApiBaseUri;
            var client = debugLog is null
                ? new RestClient(httpClient, configureSerialization: s => s.UseSystemTextJson(RestJsonOptions.Instance))
                : new RestClient(
                    httpClient,
                    false,
                    options =>
                    {
                        options.BaseUrl = BillyApiBaseUri;
                        options.Interceptors = [new BillyHttpDebugInterceptor(debugLog)];
                    },
                    s => s.UseSystemTextJson(RestJsonOptions.Instance));
            client.AddBillyAuthentication(key);
            return client;
        }

        /// <summary>
        /// Creates a Billy API client that manages its own <see cref="HttpClient"/>.
        /// Suitable for short-lived use; for long-lived scenarios prefer the <see cref="HttpClient"/> overloads.
        /// </summary>
        /// <param name="key">Billy API access token.</param>
        public static RestClient CreateBillyClient(string key)
            => CreateBillyClient(key, (Action<string>?)null);

        /// <summary>
        /// Creates a Billy API client that manages its own <see cref="HttpClient"/>, with debug logging.
        /// </summary>
        /// <param name="key">Billy API access token.</param>
        /// <param name="debugLog">Log sink that receives raw request/response text for diagnostics.</param>
        public static RestClient CreateBillyClient(string key, BillyDebugLog debugLog)
            => CreateBillyClient(key, debugLog.Write);

        /// <summary>
        /// Creates a Billy API client that manages its own <see cref="HttpClient"/>, with a custom log callback.
        /// </summary>
        /// <param name="key">Billy API access token.</param>
        /// <param name="debugLog">Callback that receives raw request/response text, or <c>null</c> to disable logging.</param>
        public static RestClient CreateBillyClient(string key, Action<string>? debugLog)
        {
            var client = debugLog is null
                ? new RestClient(BillyApiBaseUri, configureSerialization: s => s.UseSystemTextJson(RestJsonOptions.Instance))
                : new RestClient(
                    new RestClientOptions(BillyApiBaseUri) { Interceptors = [new BillyHttpDebugInterceptor(debugLog)] },
                    configureSerialization: s => s.UseSystemTextJson(RestJsonOptions.Instance));
            client.AddBillyAuthentication(key);
            return client;
        }
    }
}
