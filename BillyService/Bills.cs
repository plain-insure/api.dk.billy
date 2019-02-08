using BillyService.Models;

namespace BillyService
{
    public class Bills : Repositories.Base<Bill, BillRoot>
    {
        public Bills(RestSharp.RestClient client) : base(
            client,
            "bills/",
            (root) => root.bill,
            (root) => root.bills,
            (item) => item.id
            )
        { }
        public Bills(string key) : base(
            key,
            "bills/",
            (root) => root.bill,
            (root) => root.bills,
            (item) => item.id
            )
        { }

    }
}
