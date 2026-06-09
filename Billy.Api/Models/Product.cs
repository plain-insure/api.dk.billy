using Billy.Api.Utils;
using System.Text.Json.Serialization;

namespace Billy.Api.Models
{
    /// <summary>
    /// API response envelope for product endpoints (<c>GET /v2/products</c>, <c>POST /v2/products</c>, etc.).
    /// </summary>
    public class ProductRoot : Root
    {
        /// <summary>Single product returned by a Get request.</summary>
        public Product? Product { get; set; }

        /// <summary>List of products returned by a List or Create request.</summary>
        public List<Product>? Products { get; set; }
    }

    /// <summary>
    /// Represents a product or service that can be added to invoices and bills in Billy.
    /// Products carry default pricing, tax rules, and ledger account assignments.
    /// </summary>
    public class Product : IEntity
    {
        /// <summary>Unique identifier assigned by the Billy API.</summary>
        public string? Id { get; set; }

        /// <summary>ID of the organization this product belongs to.</summary>
        public required string OrganizationId { get; set; }

        /// <summary>Display name of the product or service.</summary>
        public required string Name { get; set; }

        /// <summary>External identifier for cross-referencing with other systems.</summary>
        public string? ExternalId { get; set; }

        /// <summary>Detailed description of the product, shown on invoice line items.</summary>
        public string? Description { get; set; }

        /// <summary>ID of the default ledger <see cref="Account"/> used when this product is invoiced.</summary>
        public required string? AccountId { get; set; }

        /// <summary>
        /// Resolved account object. Populated when sideloaded by the API.
        /// Not serialized — use <see cref="AccountId"/> when writing.
        /// </summary>
        [JsonIgnore]
        public Account? Account { get; set; }

        /// <summary>ID of the inventory <see cref="Account"/> used when tracking product stock.</summary>
        public string? InventoryAccountId { get; set; }

        /// <summary>Your internal product/SKU number for identification and search.</summary>
        public string? ProductNo { get; set; }

        /// <summary>The supplier's own product number for cross-referencing on bills.</summary>
        public string? SuppliersProductNo { get; set; }

        /// <summary>
        /// ID of the <see cref="SalesTaxRuleset"/> that determines which tax rate applies when
        /// selling this product based on the customer's location and type.
        /// </summary>
        public string? SalesTaxRulesetId { get; set; }

        /// <summary><c>true</c> if the product has been archived and should not appear in active lists.</summary>
        public bool IsArchived { get; set; }

        /// <summary><c>true</c> if inventory tracking is enabled for this product.</summary>
        public bool IsInInventory { get; set; }

        /// <summary>Unit of measure displayed on invoice lines (e.g. <c>"pcs"</c>, <c>"hours"</c>).</summary>
        public string? Unit { get; set; }

        /// <summary>ID of the image file associated with this product.</summary>
        public string? ImageId { get; set; }

        /// <summary>URL to the product image. Server-assigned; read-only.</summary>
        [JsonIgnoreOnWrite]
        public string? ImageUrl { get; set; }

        /// <summary>Timestamp when the product was created. Server-assigned; read-only.</summary>
        [JsonIgnoreOnWrite]
        public DateTime CreatedTime { get; set; }
    }
}
