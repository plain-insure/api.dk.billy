using System.Text.Json.Serialization;

namespace Billy.Api.Models
{
    /// <summary>
    /// Indicates whether a <see cref="BankPayment"/> represents an incoming or outgoing cash flow.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<CashSide>))]
    public enum CashSide
    {
        /// <summary>
        /// Debit — money received into the cash/bank account (e.g. customer payment for an invoice).
        /// </summary>
        Debit,

        /// <summary>
        /// Credit — money leaving the cash/bank account (e.g. paying a supplier bill).
        /// </summary>
        Credit
    }
}
