using RestSharp;

namespace BillyService.Utils
{
    public static class ClientExtensions
    {
        public static void AddBillyAuthentication(this RestSharp.RestClient client, string key)
        {

            client.AddDefaultHeader("X-Access-Token", key);
        }

        public static RestClient CreateBillyClient(string key)
        {
            var client = new RestClient("https://api.billysbilling.com/v2/");
            client.AddBillyAuthentication(key);
            return client;
        }
    }
}
