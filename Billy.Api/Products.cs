using Billy.Api.Models;

namespace Billy.Api
{
    /// <summary>
    /// Repository for the Billy <c>/v2/products</c> endpoint. Supports full CRUD — Get, List, Create,
    /// Update, and Delete.
    /// </summary>
    public class Products : Repositories.BaseWithDelete<Product, ProductRoot>
    {
        /// <inheritdoc cref="Repositories.Base{T,TRoot}(RestSharp.RestClient)"/>
        public Products(RestSharp.RestClient? client, string? key) : base(
            client, key)
        {
        }

        /// <inheritdoc cref="Repositories.Base{T,TRoot}(RestSharp.RestClient)"/>
        public Products(RestSharp.RestClient client) : this(client, null) { }

        /// <inheritdoc cref="Repositories.Base{T,TRoot}(string)"/>
        public Products(string key) : this(null, key) { }
    }
}
