using Billy.Api.Models;

namespace Billy.Api
{
    public class Organizations : Repositories.Base<Organization, OrganizationRoot>
    {
        public Organizations(RestSharp.RestClient client) : base(
            client,
            "organizations/",
            (root) => root?.Organization,
            (root) => root?.Organizations,
            (item) => item?.Id
            )
        { }

        public Organizations(string key) : base(
            key,
            "organizations/",
            (root) => root?.Organization,
            (root) => root?.Organizations,
            (item) => item?.Id
            )
        { }

    }
}
