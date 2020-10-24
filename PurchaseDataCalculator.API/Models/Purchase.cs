namespace PurchaseDataCalculatorAPI.Models
{
    public class Purchase
    {
        /// <summary>
        /// Valid VAT rate values are : 10, 13, 20
        /// </summary>
        public decimal VatRate { get; set; }
        public decimal? NetAmount { get; set; }
        public decimal? GrossAmount { get; set; }
        public decimal? VatAmount { get; set; }
    }
}
