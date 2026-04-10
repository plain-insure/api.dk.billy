using Billy.Api.Models;

namespace Billy.Api
{
    public class Accounts : Repositories.Base<Account, AccountRoot>
    {


        private Accounts(RestSharp.RestClient? client, string? key) : base(
            client, key,
            "accounts/",
            (root) => root?.Account,
            (root) => root?.Accounts,
            (item) => item?.Id
            )
        {
            AddSideload(r => r.AccountGroups, a => a.Group); // Same as:
            //AddSideload(r => r.AccountGroups, a => a.Group, a => a.GroupId);

            AddSideload(r => r.Organization); // Same as:
            //AddSideload(r => r.Organizations, a => a.Organization, a => a.OrganizationId);
        }

        public Accounts(RestSharp.RestClient client) : this(client, null) { }

        public Accounts(string key) : this(null, key) { }


    }
}
