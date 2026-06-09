using Billy.Api.Models;

namespace Billy.Api
{
    /// <summary>
    /// Read-only repository for the Billy <c>/v2/accounts</c> endpoint. Supports Get and List.
    /// Account groups and the parent organization are sideloaded automatically on all responses.
    /// </summary>
    public class Accounts : Repositories.Base<Account, AccountRoot>
    {
        private Accounts(RestSharp.RestClient? client, string? key) : base(
            client, key
            )
        {
            AddSideload(r => r.AccountGroups, a => a.Group, "accounts.group");
            AddSideload(r => r.Organization, "account.organization");
        }

        /// <inheritdoc cref="Repositories.Base{T,TRoot}(RestSharp.RestClient)"/>
        public Accounts(RestSharp.RestClient client) : this(client, null) { }

        /// <inheritdoc cref="Repositories.Base{T,TRoot}(string)"/>
        public Accounts(string key) : this(null, key) { }
    }
}
