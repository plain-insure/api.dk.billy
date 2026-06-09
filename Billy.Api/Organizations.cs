using Billy.Api.Models;

namespace Billy.Api
{
    /// <summary>
    /// Read-only repository for the Billy <c>/v2/organizations</c> endpoint. Supports Get and List.
    /// Each API access token is scoped to a single organization.
    /// </summary>
    public class Organizations : Repositories.Base<Organization, OrganizationRoot>
    {
        /// <inheritdoc cref="Repositories.Base{T,TRoot}(RestSharp.RestClient)"/>
        public Organizations(RestSharp.RestClient client) : base(
            client
            )
        { }

        /// <inheritdoc cref="Repositories.Base{T,TRoot}(string)"/>
        public Organizations(string key) : base(
            key
            )
        { }
    }
}
