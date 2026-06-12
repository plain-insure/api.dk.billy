using System.Text.Json.Serialization;

namespace Billy.Api.Models
{
    /// <summary>
    /// A single line item on an <see cref="Invoice"/>, representing a product or service being sold.
    /// </summary>
    public class InvoiceLine
    {
        /// <summary>Unique identifier of the line item.</summary>
        public string Id { get; set; }

        /// <summary>ID of the <see cref="Invoice"/> this line belongs to.</summary>
        public string InvoiceId { get; set; }

        /// <summary>ID of the <see cref="Product"/> associated with this line, if any.</summary>
        public string ProductId { get; set; }

        /// <summary>Line item description printed on the invoice.</summary>
        public string Description { get; set; }

        /// <summary>Number of units sold on this line.</summary>
        public decimal Quantity { get; set; }

        /// <summary>Price per unit in the invoice's currency.</summary>
        public decimal UnitPrice { get; set; }

        /// <summary>Total net amount for this line (excluding tax), in the invoice's currency.</summary>
        public decimal Amount { get; set; }

        /// <summary>Tax amount for this line, in the invoice's currency.</summary>
        public decimal? Tax { get; set; }

        /// <summary>ID of the <see cref="TaxRate"/> applied to this line.</summary>
        public string? TaxRateId { get; set; }

        /// <summary>Text label of the discount applied to this line, if any.</summary>
        public string? DiscountText { get; set; }

        /// <summary>
        /// Discount mode: <c>"amount"</c> for fixed amount discount, <c>"percent"</c> for percentage discount.
        /// </summary>
        public DiscountMode? DiscountMode { get; set; }

        /// <summary>The discount value, interpreted according to <see cref="DiscountMode"/>.</summary>
        public decimal? DiscountValue { get; set; }

        /// <summary>Display order of this line relative to other lines on the invoice (lower = first).</summary>
        public int Priority { get; set; }
    }

}
