using Billy.Api.Models;

namespace Billy.Api
{
    public class Accounts : Repositories.Base<Account, AccountRoot>
    {
        public Accounts(RestSharp.RestClient client) : base(
            client,
            "accounts/",
            (root) => root?.Account,
            (root) => root?.Accounts,
            (item) => item?.Id
            )
        { }

        public Accounts(string key) : base(
            key,
            "accounts/",
            (root) => root?.Account,
            (root) => root?.Accounts,
            (item) => item?.Id
            )
        { }
    }
}
