using Billy.Api.Models;

namespace Billy.Api
{
    public class Bills : Repositories.Base<Bill, BillRoot>
    {
        public Bills(RestSharp.RestClient client) : base(
            client,
            "bills/",
            (root) => root?.Bill,
            (root) => root?.Bills,
            (item) => item?.Id
            )
        { }

        public Bills(string key) : base(
            key,
            "bills/",
            (root) => root?.Bill,
            (root) => root?.Bills,
            (item) => item?.Id
            )
        { }

    }
}
