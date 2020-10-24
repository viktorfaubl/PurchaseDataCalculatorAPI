namespace PurchaseDataCalculatorAPI.Interfaces
{
    public interface IGrossCalculator
    {
        public decimal CalculateGross(decimal? netAmount, decimal vatRate);
    }
}
