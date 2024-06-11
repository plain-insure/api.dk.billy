using Billy.Api.Models;

namespace Billy.Api
{
    public class AccountGroups : Repositories.Base<AccountGroup, AccountGroupRoot>
    {
        private AccountGroups(RestSharp.RestClient? client, string? key) : base(
            client, key,
            "accounts/",
            (root) => root?.AccountGroup,
            (root) => root?.AccountGroups,
            (item) => item?.Id
            )
        {

        }

        public AccountGroups(RestSharp.RestClient client) : this(client, null) { }

        public AccountGroups(string key) : this(null, key) { }

    }
}
