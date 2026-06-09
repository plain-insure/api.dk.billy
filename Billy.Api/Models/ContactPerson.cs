namespace Billy.Api.Models
{
    /// <summary>
    /// A named contact person associated with a <see cref="Contact"/>.
    /// Contact persons can receive invoice emails via the Billy API.
    /// </summary>
    public class ContactPerson
    {
        /// <summary>Full name of the contact person.</summary>
        public string Name { get; set; }

        /// <summary>Email address used for invoice delivery.</summary>
        public string Email { get; set; }
    }
}
