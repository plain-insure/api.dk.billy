using Billy.Api.Utils;
using System.Text.Json.Serialization;

namespace Billy.Api.Models
{
    /// <summary>
    /// API response envelope for contact endpoints (<c>GET /v2/contacts</c>, <c>POST /v2/contacts</c>, etc.).
    /// </summary>
    public class ContactRoot : Root
    {
        /// <summary>List of contacts returned by a List or Create request.</summary>
        public List<Contact>? Contacts { get; set; }

        /// <summary>Single contact returned by a Get request.</summary>
        public Contact? Contact { get; set; }

        /// <summary>Sideloaded countries. Populated when <see cref="Contacts.SideloadCountry"/> is enabled.</summary>
        public List<Country>? Countries { get; set; }

        /// <summary>Sideloaded currencies associated with the contacts.</summary>
        public List<Currency>? Currencies { get; set; }

        /// <summary>Sideloaded locales. Populated when <see cref="Contacts.SideloadLocale"/> is enabled.</summary>
        public List<Locale>? Locales { get; set; }
    }

    /// <summary>
    /// Represents a customer or supplier contact in Billy.
    /// A contact can be a company or an individual, and may act as both a customer and a supplier.
    /// </summary>
    public class Contact : IEntity
    {
        /// <summary>Timestamp when the contact was created. Server-assigned; read-only.</summary>
        [JsonIgnoreOnWrite]
        public virtual DateTime CreatedTime { get; set; }

        /// <summary>
        /// Resolved country object. Populated when <see cref="Contacts.SideloadCountry"/> is enabled.
        /// Not serialized — use <see cref="CountryId"/> when writing.
        /// </summary>
        [JsonIgnore]
        public Country Country { get; set; }

        /// <summary>
        /// Resolved currency object. Populated when sideloaded by the API.
        /// Not serialized — use <see cref="CurrencyId"/> when writing.
        /// </summary>
        [JsonIgnore]
        public Currency Currency { get; set; }

        /// <summary>
        /// Resolved locale object. Populated when <see cref="Contacts.SideloadLocale"/> is enabled.
        /// Not serialized — use <see cref="LocaleId"/> when writing.
        /// </summary>
        [JsonIgnore]
        public Locale Locale { get; set; }

        /// <summary>Named individuals associated with this contact who can receive invoice emails.</summary>
        public List<ContactPerson> ContactPersons { get; set; }

        /// <summary>Unique identifier assigned by the Billy API.</summary>
        public string? Id { get; set; }

        /// <summary>Contact type: <c>"company"</c> or <c>"person"</c>.</summary>
        public required string Type { get; set; }

        /// <summary>ID of the organization this contact belongs to.</summary>
        public string OrganizationId { get; set; }

        /// <summary>Display name of the contact (company name or person's full name).</summary>
        public required string Name { get; set; }

        /// <summary>ISO 3166-1 alpha-2 country code for the contact's address (e.g. <c>"DK"</c>).</summary>
        public required string CountryId { get; set; }

        /// <summary>Street address of the contact.</summary>
        public string Street { get; set; }

        /// <summary>City identifier, used for countries with API-managed city lists.</summary>
        public string CityId { get; set; }

        /// <summary>Free-text city name, used when the country does not have a finite city list.</summary>
        public string CityText { get; set; }

        /// <summary>State identifier, used for countries that have states.</summary>
        public object StateId { get; set; }

        /// <summary>Free-text state name.</summary>
        public string StateText { get; set; }

        /// <summary>Zip code identifier, used for countries with API-managed zip code lists.</summary>
        public string ZipcodeId { get; set; }

        /// <summary>Free-text zip/postal code.</summary>
        public string ZipcodeText { get; set; }

        /// <summary>Internal contact number used for your own reference.</summary>
        public string ContactNo { get; set; }

        /// <summary>Primary phone number of the contact.</summary>
        public string Phone { get; set; }

        /// <summary>Fax number of the contact.</summary>
        public string Fax { get; set; }

        /// <summary>ISO 4217 currency code for this contact's default invoicing currency.</summary>
        public string? CurrencyId { get; set; }

        /// <summary>
        /// Company registration number (CVR number in Denmark, VAT number in other EU countries).
        /// Used for B2B invoicing.
        /// </summary>
        public string RegistrationNo { get; set; }

        /// <summary>EAN (European Article Number) used for electronic invoicing in the public sector.</summary>
        public string Ean { get; set; }

        /// <summary>IETF locale tag controlling the language of documents sent to this contact.</summary>
        public string? LocaleId { get; set; }

        /// <summary><c>true</c> if this contact is a customer (can be invoiced).</summary>
        public bool IsCustomer { get; set; }

        /// <summary><c>true</c> if this contact is a supplier (can send bills).</summary>
        public bool IsSupplier { get; set; }

        /// <summary>
        /// Default payment terms mode for invoices to this contact:
        /// <c>"net"</c>, <c>"dueDate"</c>, <c>"paid"</c>, etc.
        /// </summary>
        public string PaymentTermsMode { get; set; }

        /// <summary>Number of days used with <see cref="PaymentTermsMode"/> to compute the due date.</summary>
        public string PaymentTermsDays { get; set; }

        /// <summary>
        /// Access code that allows the contact to view their invoices in the Billy customer portal
        /// without logging in.
        /// </summary>
        public string AccessCode { get; set; }

        /// <summary>
        /// How invoice attachments are delivered: <c>"link"</c> to send a download link,
        /// or <c>"attachment"</c> to attach the PDF directly to the email.
        /// </summary>
        public string EmailAttachmentDeliveryMode { get; set; }

        /// <summary><c>true</c> if the contact has been archived and should not appear in active lists.</summary>
        public bool IsArchived { get; set; }

        /// <summary><c>true</c> if this contact is exempt from sales tax on all invoices.</summary>
        public bool IsSalesTaxExempt { get; set; }
    }


}
