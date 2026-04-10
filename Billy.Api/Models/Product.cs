using Billy.Api.Utils;
using System.Text.Json.Serialization;

namespace Billy.Api.Models
{
    public class ProductIdList
    {
        public List<string>? Products { get; set; }
    }

    public class ProductRoot : Root<ProductIdList>
    {
        public Product? Product { get; set; }
        public List<Product>? Products { get; set; }
    }

    public class Product : IEntity
    {
        public string? Id { get; set; }
        public required string OrganizationId { get; set; }
        public required string Name { get; set; }
        public string? ExternalId { get; set; }
        public string? Description { get; set; }

        public required string? AccountId { get; set; }

        [JsonIgnore]
        public Account? Account { get; set; }

        public string? InventoryAccountId { get; set; }
        public string? ProductNo { get; set; }
        public string? SuppliersProductNo { get; set; }
        public string? SalesTaxRulesetId { get; set; }
        public bool IsArchived { get; set; }
        public bool IsInInventory { get; set; }
        public string? Unit { get; set; }
        public string? ImageId { get; set; }

        [JsonIgnoreOnWrite]
        public string? ImageUrl { get; set; }

        [JsonIgnoreOnWrite]
        public DateTime CreatedTime { get; set; }
    }
}
