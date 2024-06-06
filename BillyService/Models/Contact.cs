using Billy.Models;

namespace BillyService.Models
{

    public class ContactRoot
    {
        public Meta Meta { get; set; }
        public Contact Contact { get; set; }
        public List<Contact> Contacts { get; set; }
    }

    public class PostContact
    {
        public string Type { get; set; }
        public string OrganizationId { get; set; }
        public string Name { get; set; }
        public string CountryId { get; set; }
        public string Street { get; set; }
        public string CityText { get; set; }
        public object ZipcodeId { get; set; }
        public string ZipcodeText { get; set; }
        public string Phone { get; set; }
        public string RegistrationNo { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsSupplier { get; set; }
        public List<ContactPerson> ContactPersons { get; set; }
    }

    public class Contact
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string OrganizationId { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Name { get; set; }
        public string CountryId { get; set; }
        public string Street { get; set; }
        public string CityId { get; set; }
        public string CityText { get; set; }
        public object StateId { get; set; }
        public string StateText { get; set; }
        public string ZipcodeId { get; set; }
        public string ZipcodeText { get; set; }
        public string ContactNo { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public object CurrencyId { get; set; }
        public string RegistrationNo { get; set; }
        public string Ean { get; set; }
        public object LocaleId { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsSupplier { get; set; }
        public object PaymentTermsMode { get; set; }
        public object PaymentTermsDays { get; set; }
        public string AccessCode { get; set; }
        public object EmailAttachmentDeliveryMode { get; set; }
        public bool IsArchived { get; set; }
        public bool IsSalesTaxExempt { get; set; }
        public List<ContactPerson> ContactPersons { get; set; }
    }

}
