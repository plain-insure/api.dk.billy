using System.Text.Json.Serialization;

namespace Billy.Api.Models
{
    /// <summary>
    /// The root of the object that is returned when an account is created, updated, or retrieved.
    /// </summary>
    public class AccountRoot
    {
        public Meta Meta { get; set; }
        public Account Account { get; set; }
        public List<Account> Accounts { get; set; }
    }
    public class Account
    {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string NatureId { get; set; }
        [JsonConverter(typeof(Converters.BillyDateConverter))]
        public DateTime CreatedTime { get; set; }
        [JsonConverter(typeof(Converters.BillyDateConverter))]
        public DateTime UpdatedTime { get; set; }
        public object PredefinedAccountId { get; set; }
        public object PublicAccountId { get; set; }
        public string Name { get; set; }
        public string GroupId { get; set; }
        public int AccountNo { get; set; }
        public string SystemRole { get; set; }
        public bool IsPaymentEnabled { get; set; }
        public bool IsBankAccount { get; set; }
        public string Description { get; set; }
        public bool IsArchived { get; set; }
        public string CurrencyId { get; set; }
        public string TaxRateId { get; set; }
        public string BankId { get; set; }
        public string BankName { get; set; }
        public string BankRoutingNo { get; set; }
        public string BankAccountNo { get; set; }
        public string BankSwift { get; set; }
        public string BankIban { get; set; }
    }
}