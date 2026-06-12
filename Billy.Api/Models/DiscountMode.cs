namespace Billy.Api.Models
{
    /// <summary>
    /// Defines how to interpret the discount value on an <see cref="InvoiceLine"/>: either as a percentage or a fixed amount.
    /// </summary>
    public enum DiscountMode
    {
        /// <summary>Percentage discount (e.g. 10 for 10% off).</summary>
        Percent,
        /// <summary>Fixed amount discount in the invoice's currency (e.g. 15 for $15 off).</summary>
        Cash
    }

}
