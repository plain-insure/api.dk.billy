using Billy.Api.Models;

namespace Billy.Api
{
    public class Daybooks : Repositories.BaseWithDelete<Daybook, DaybookRoot>
    {
        public Daybooks(RestSharp.RestClient client) : base(
            client
        )
        { }

        public Daybooks(string key) : base(
            key
        )
        { }
    }
}
