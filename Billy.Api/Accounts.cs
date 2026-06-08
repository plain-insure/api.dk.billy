using Billy.Api.Models;

namespace Billy.Api
{
    public class Accounts : Repositories.Base<Account, AccountRoot>
    {


        private Accounts(RestSharp.RestClient? client, string? key) : base(
            client, key
            )
        {
            AddSideload(r => r.AccountGroups, a => a.Group, "accounts.group"); // Same as:
            //AddSideload(r => r.AccountGroups, a => a.Group, a => a.GroupId);

            AddSideload(r => r.Organization, "account.organization"); // Same as:
            //AddSideload(r => r.Organizations, a => a.Organization, a => a.OrganizationId);
        }

        public Accounts(RestSharp.RestClient client) : this(client, null) { }

        public Accounts(string key) : this(null, key) { }


    }
}
