using Billy.Api.Utils;
using System.Text.Json.Serialization;

namespace Billy.Api.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter<CashSide>))]
    public enum CashSide
    {
        debit,
        credit
    }

    public class BankPaymentIdList
    {
        public List<string>? BankPayments { get; set; }
    }

    public class BankPaymentRoot : Root<BankPaymentIdList>
    {
        public BankPayment? BankPayment { get; set; }
        public List<BankPayment>? BankPayments { get; set; }
    }

    public class BankPayment : IEntity
    {
        public string? Id { get; set; }
        public string OrganizationId { get; set; }
        public string? ContactId { get; set; }

        [JsonIgnoreOnWrite]
        public DateTime CreatedTime { get; set; }

        [JsonConverter(typeof(Converters.BillyDateConverter))]
        public DateTime EntryDate { get; set; }

        public double CashAmount { get; set; }
        public CashSide CashSide { get; set; }
        public string CashAccountId { get; set; }
        public double? CashExchangeRate { get; set; }
        public string? SubjectCurrencyId { get; set; }
        public double? FeeAmount { get; set; }
        public string? FeeAccountId { get; set; }
        public bool? IsVoided { get; set; }

        public List<BankPaymentAssociation>? Associations { get; set; }

        [JsonIgnoreOnWrite]
        public List<BankPaymentContactBalancePosting>? ContactBalancePostings { get; set; }
    }

    public class BankPaymentAssociation
    {
        public string? SubjectReference { get; set; }
    }

    public class BankPaymentContactBalancePosting
    {
        public string? Id { get; set; }
        public string? ContactId { get; set; }
        public string? CurrencyId { get; set; }
        public double? Amount { get; set; }
        public string? Side { get; set; }
    }
}
