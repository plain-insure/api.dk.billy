using Billy.Api.Models;

namespace Billy.Api
{
    public class DaybookTransactions : Repositories.BaseWithDelete<DaybookTransaction, DaybookTransactionRoot>
    {
        public DaybookTransactions(RestSharp.RestClient client) : base(
            client
        )
        {
            this.AddSideload(r => r.DaybookTransactionLines, d => d.Lines);
        }

        public DaybookTransactions(string key) : base(
            key
        )
        {
            this.AddSideload(r => r.DaybookTransactionLines, d => d.Lines);
        }
    }
}
