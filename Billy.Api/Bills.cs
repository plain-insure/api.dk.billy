using Billy.Api.Models;

namespace Billy.Api
{
    public class Bills : Repositories.BaseWithDelete<Bill, BillRoot>
    {
        public Bills(RestSharp.RestClient client) : base(
            client,
            "bills/",
            (root) => root?.Bill,
            (root) => root?.Bills,
            (item) => item?.Id,
            (item) => new BillRoot { Bill = item },
            (root) => root?.Meta?.DeletedRecords?.Bills?.FirstOrDefault()
            )
        { }

        public Bills(string key) : base(
            key,
            "bills/",
            (root) => root?.Bill,
            (root) => root?.Bills,
            (item) => item?.Id,
            (item) => new BillRoot { Bill = item },
            (root) => root?.Meta?.DeletedRecords?.Bills?.FirstOrDefault()
            )
        { }
    }
}
