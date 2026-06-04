using Billy.Api.Models;

namespace Billy.Api
{
    public class DaybookTransactions : Repositories.BaseWithDelete<DaybookTransaction, DaybookTransactionRoot>
    {
        public DaybookTransactions(RestSharp.RestClient client) : base(
            client,
            "daybookTransactions/",
            (root) => root?.DaybookTransaction,
            (root) => root?.DaybookTransactions,
            (item) => item?.Id,
            (item) => new DaybookTransactionRoot { DaybookTransaction = item },
            (root) => root?.Meta?.DeletedRecords?.DaybookTransactions?.FirstOrDefault()
        )
        { }

        public DaybookTransactions(string key) : base(
            key,
            "daybookTransactions/",
            (root) => root?.DaybookTransaction,
            (root) => root?.DaybookTransactions,
            (item) => item?.Id,
            (item) => new DaybookTransactionRoot { DaybookTransaction = item },
            (root) => root?.Meta?.DeletedRecords?.DaybookTransactions?.FirstOrDefault()
        )
        { }
    }
}
