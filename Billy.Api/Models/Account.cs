using System.Text.Json.Serialization;

namespace Billy.Api.Models
{
    /// <summary>
    /// API response envelope for account endpoints (<c>GET /v2/accounts</c>, etc.).
    /// </summary>
    public class AccountRoot : Root
    {
        /// <summary>Single account returned by a Get request.</summary>
        public Account Account { get; set; }

        /// <summary>List of accounts returned by a List request.</summary>
        public List<Account> Accounts { get; set; }

        /// <summary>Sideloaded account groups. Populated by the <see cref="Accounts"/> repository.</summary>
        public List<AccountGroup> AccountGroups { get; set; }

        /// <summary>Sideloaded organizations. Populated by the <see cref="Accounts"/> repository.</summary>
        public List<Organization> Organizations { get; set; }
    }

    /// <summary>
    /// Represents a ledger account (chart of accounts entry) in Billy.
    /// Accounts are read-only through this library — use the Billy web app to create or modify them.
    /// </summary>
    public class Account : IEntity
    {
        /// <summary>Unique identifier assigned by the Billy API.</summary>
        public string Id { get; set; }

        /// <summary>ID of the organization this account belongs to.</summary>
        public string? OrganizationId { get; set; }

        /// <summary>
        /// Resolved organization object. Populated by the <see cref="Accounts"/> repository.
        /// Not serialized — use <see cref="OrganizationId"/> when writing.
        /// </summary>
        [JsonIgnore]
        public Organization? Organization { get; set; }

        /// <summary>
        /// ID of the account nature (asset, liability, equity, income, or expense).
        /// Determines which side of the balance sheet or income statement the account appears on.
        /// </summary>
        public string NatureId { get; set; }

        /// <summary>Timestamp when the account was created.</summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>Timestamp when the account was last modified.</summary>
        public DateTime UpdatedTime { get; set; }

        /// <summary>ID of the predefined account template this account was created from, if any.</summary>
        public string? PredefinedAccountId { get; set; }

        /// <summary>ID used to reference this account in shared (cross-organization) contexts.</summary>
        public string? PublicAccountId { get; set; }

        /// <summary>Display name of the account.</summary>
        public string Name { get; set; }

        /// <summary>ID of the <see cref="AccountGroup"/> this account belongs to.</summary>
        public string GroupId { get; set; }

        /// <summary>
        /// Resolved account group object. Populated by the <see cref="Accounts"/> repository.
        /// Not serialized — use <see cref="GroupId"/> when writing.
        /// </summary>
        [JsonIgnore]
        public AccountGroup? Group { get; set; }

        /// <summary>
        /// The account number shown in the chart of accounts (e.g. <c>1000</c>, <c>6600</c>).
        /// </summary>
        public int AccountNo { get; set; }

        /// <summary>
        /// Billy system role for this account (e.g. <c>"bankAccount"</c>, <c>"vatPayable"</c>).
        /// System roles indicate how Billy uses the account internally.
        /// </summary>
        public string SystemRole { get; set; }

        /// <summary>
        /// <c>true</c> if this account can be selected as a payment account when recording payments.
        /// </summary>
        public bool IsPaymentEnabled { get; set; }

        /// <summary><c>true</c> if this account represents a bank account.</summary>
        public bool IsBankAccount { get; set; }

        /// <summary>Optional description of the account's purpose.</summary>
        public string Description { get; set; }

        /// <summary><c>true</c> if the account has been archived and is no longer in active use.</summary>
        public bool IsArchived { get; set; }

        /// <summary>ISO 4217 currency code if this account is denominated in a foreign currency.</summary>
        public string CurrencyId { get; set; }

        /// <summary>ID of the default <see cref="TaxRate"/> applied when posting to this account.</summary>
        public string TaxRateId { get; set; }

        /// <summary>ID of the bank integration linked to this account.</summary>
        public string BankId { get; set; }

        /// <summary>Name of the bank institution associated with this account.</summary>
        public string BankName { get; set; }

        /// <summary>Bank routing/sort code (e.g. Danish registration number).</summary>
        public string BankRoutingNo { get; set; }

        /// <summary>Bank account number.</summary>
        public string BankAccountNo { get; set; }

        /// <summary>SWIFT/BIC code of the bank.</summary>
        public string BankSwift { get; set; }

        /// <summary>IBAN number of the bank account.</summary>
        public string BankIban { get; set; }
    }
}
