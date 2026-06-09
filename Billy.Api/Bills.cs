using Billy.Api.Models;

namespace Billy.Api
{
    /// <summary>
    /// Repository for the Billy <c>/v2/bills</c> endpoint. Supports full CRUD — Get, List, Create,
    /// Update, and Delete. Use <see cref="SideloadLines"/> to include line items in responses.
    /// </summary>
    public class Bills : Repositories.BaseWithDelete<Bill, BillRoot>
    {
        /// <inheritdoc cref="Repositories.Base{T,TRoot}(RestSharp.RestClient)"/>
        public Bills(RestSharp.RestClient client) : base(client) { }

        /// <inheritdoc cref="Repositories.Base{T,TRoot}(string)"/>
        public Bills(string key) : base(key) { }

        /// <summary>
        /// Enables sideloading of <see cref="Bill.Lines"/> in all subsequent Get and List responses.
        /// Without calling this, <see cref="Bill.Lines"/> will be <c>null</c>.
        /// </summary>
        public void SideloadLines()
        {
            this.AddSideload(r => r.BillLines, b => b.Lines);
        }
    }
}
