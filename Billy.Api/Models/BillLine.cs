namespace Billy.Api.Models
{
    /// <summary>
    /// A single line item on a <see cref="Bill"/>, representing a purchased product or expense category.
    /// </summary>
    public class BillLine : IEntity
    {
        /// <summary>Unique identifier of the line item.</summary>
        public string? Id { get; set; }

        /// <summary>ID of the <see cref="Bill"/> this line belongs to.</summary>
        public string? BillId { get; set; }

        /// <summary>ID of the ledger <see cref="Account"/> to post this line to.</summary>
        public string AccountId { get; set; }

        /// <summary>ID of the <see cref="TaxRate"/> applied to this line, or <c>null</c> if tax-exempt.</summary>
        public string? TaxRateId { get; set; }

        /// <summary>Description of the line item.</summary>
        public string? Description { get; set; }

        /// <summary>Net amount for this line (excluding tax), in the bill's currency.</summary>
        public decimal Amount { get; set; }

        /// <summary>Tax amount for this line, in the bill's currency.</summary>
        public decimal Tax { get; set; }

        /// <summary>Display order of this line relative to other lines on the same bill (lower = first).</summary>
        public int Priority { get; set; }
    }
}
