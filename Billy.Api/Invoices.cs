using Billy.Api.Models;

namespace Billy.Api
{
    /// <summary>
    /// Repository for the Billy <c>/v2/invoices</c> endpoint. Supports full CRUD — Get, List, Create,
    /// Update, and Delete.
    /// </summary>
    public class Invoices : Repositories.BaseWithDelete<Invoice, InvoiceRoot>
    {
        /// <inheritdoc cref="Repositories.Base{T,TRoot}(RestSharp.RestClient)"/>
        public Invoices(RestSharp.RestClient client) : base(
            client)
        { }

        /// <inheritdoc cref="Repositories.Base{T,TRoot}(string)"/>
        public Invoices(string key) : base(
            key)
        { }

        /// <summary>
        /// Returns the most recently approved invoice for the given contact,
        /// or <c>null</c> if the contact has no approved invoices.
        /// </summary>
        /// <param name="contactId">ID of the contact whose latest invoice to retrieve.</param>
        public Invoice MostRecent(string contactId)
        {
            var invoices = List(new { contactId }, i => i.ApprovedTime, SortOrder.DESC, 1);
            return invoices.FirstOrDefault();
        }
    }
}
