using Billy.Api.Models;

namespace Billy.Api
{
    public class Products : Repositories.BaseWithDelete<Product, ProductRoot>
    {
        public Products(RestSharp.RestClient? client, string? key) : base(
            client, key)
        {
        }

        public Products(RestSharp.RestClient client) : this(client, null) { }

        public Products(string key) : this(null, key) { }
    }
}
