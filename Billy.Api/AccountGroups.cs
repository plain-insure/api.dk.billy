using Billy.Api.Models;

namespace Billy.Api
{
    /// <summary>
    /// Read-only repository for the Billy <c>/v2/accountGroups</c> endpoint. Supports Get and List.
    /// </summary>
    public class AccountGroups : Repositories.Base<AccountGroup, AccountGroupRoot>
    {
        private AccountGroups(RestSharp.RestClient? client, string? key) : base(
            client, key
            )
        {
        }

        /// <inheritdoc cref="Repositories.Base{T,TRoot}(RestSharp.RestClient)"/>
        public AccountGroups(RestSharp.RestClient client) : this(client, null) { }

        /// <inheritdoc cref="Repositories.Base{T,TRoot}(string)"/>
        public AccountGroups(string key) : this(null, key) { }
    }
}
