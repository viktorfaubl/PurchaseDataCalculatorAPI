namespace PurchaseDataCalculatorAPI.Interfaces
{
    public interface IVatCalculator
    {
        public decimal CalculateVat(decimal? grossAmount, decimal vatRate);
    }
}
