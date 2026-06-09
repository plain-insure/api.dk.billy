using Billy.Api.Models;

namespace Billy.Api
{
    /// <summary>
    /// Repository for the Billy <c>/v2/bankPayments</c> endpoint. Supports Create, Get, and List.
    /// Use the <see cref="Actions.BankPaymentActions"/> extension methods (<c>PayInvoice</c> /
    /// <c>PayBill</c>) for a simpler way to mark invoices and bills as paid.
    /// </summary>
    public class BankPayments : Repositories.BaseWithCreate<BankPayment, BankPaymentRoot>
    {
        /// <inheritdoc cref="Repositories.Base{T,TRoot}(RestSharp.RestClient)"/>
        public BankPayments(RestSharp.RestClient client) : base(client) { }

        /// <inheritdoc cref="Repositories.Base{T,TRoot}(string)"/>
        public BankPayments(string key) : base(key) { }
    }
}
