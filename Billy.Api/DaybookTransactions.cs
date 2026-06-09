using Billy.Api.Models;

namespace Billy.Api
{
    /// <summary>
    /// Repository for the Billy <c>/v2/daybookTransactions</c> endpoint. Supports full CRUD — Get,
    /// List, Create, Update, and Delete. Transaction lines are sideloaded automatically on all responses.
    /// </summary>
    public class DaybookTransactions : Repositories.BaseWithDelete<DaybookTransaction, DaybookTransactionRoot>
    {
        /// <inheritdoc cref="Repositories.Base{T,TRoot}(RestSharp.RestClient)"/>
        public DaybookTransactions(RestSharp.RestClient client) : base(
            client
        )
        {
            this.AddSideload(r => r.DaybookTransactionLines, d => d.Lines);
        }

        /// <inheritdoc cref="Repositories.Base{T,TRoot}(string)"/>
        public DaybookTransactions(string key) : base(
            key
        )
        {
            this.AddSideload(r => r.DaybookTransactionLines, d => d.Lines);
        }
    }
}
