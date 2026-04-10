using Billy.Api.Models;

namespace Billy.Api
{
    public class Products : Repositories.BaseWithDelete<Product, ProductRoot>
    {
        public Products(RestSharp.RestClient? client, string? key) : base(
            client, key,
            "products/",
            (root) => root?.Product,
            (root) => root?.Products,
            (item) => item?.Id,
            (item) => new ProductRoot { Product = item },
            (root) => root?.Meta?.DeletedRecords?.Products?.FirstOrDefault()
            )
        {
        }

        public Products(RestSharp.RestClient client) : this(client, null) { }

        public Products(string key) : this(null, key) { }
    }
}
