namespace Billy.Models
{
    public class InvoiceLine
    {
        public string Id { get; set; }
        public string InvoiceId { get; set; }
        public string ProductId { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
        public int Amount { get; set; }
        public double Tax { get; set; }
        public string TaxRateId { get; set; }
        public object DiscountText { get; set; }
        public object DiscountMode { get; set; }
        public object DiscountValue { get; set; }
        public int Priority { get; set; }
    }
}
