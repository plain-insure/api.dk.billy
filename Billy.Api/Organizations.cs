using Billy.Api.Models;

namespace Billy.Api
{
    public class Organizations : Repositories.Base<Organization, OrganizationRoot>
    {
        public Organizations(RestSharp.RestClient client) : base(
            client
            )
        { }

        public Organizations(string key) : base(
            key
            )
        { }

    }
}
