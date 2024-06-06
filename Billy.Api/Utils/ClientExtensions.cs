using RestSharp;

namespace Billy.Api.Utils
{
    public static class ClientExtensions
    {
        public static void AddBillyAuthentication(this RestSharp.RestClient client, string key)
        {
            client.AddDefaultHeader("X-Access-Token", key);
        }

        public static RestClient CreateBillyClient(HttpClient httpClient, string key)
        {
            httpClient.BaseAddress = new Uri("https://api.billysbilling.com/v2/");
            var client = new RestClient(httpClient);
            client.AddBillyAuthentication(key);
            return client;
        }

        public static RestClient CreateBillyClient(string key)
        {
            var client = new RestClient("https://api.billysbilling.com/v2/");
            client.AddBillyAuthentication(key);
            return client;
        }
    }
}
