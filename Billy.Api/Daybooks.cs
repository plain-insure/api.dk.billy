using Billy.Api.Models;

namespace Billy.Api
{
    /// <summary>
    /// Repository for the Billy <c>/v2/daybooks</c> endpoint. Supports full CRUD — Get, List, Create,
    /// Update, and Delete.
    /// </summary>
    public class Daybooks : Repositories.BaseWithDelete<Daybook, DaybookRoot>
    {
        /// <inheritdoc cref="Repositories.Base{T,TRoot}(RestSharp.RestClient)"/>
        public Daybooks(RestSharp.RestClient client) : base(
            client
        )
        { }

        /// <inheritdoc cref="Repositories.Base{T,TRoot}(string)"/>
        public Daybooks(string key) : base(
            key
        )
        { }
    }
}
