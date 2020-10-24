namespace PurchaseDataCalculatorAPI.Interfaces
{
    public interface INetCalculator
    {
        public decimal CalculateNet(decimal? grossAmount, decimal vatRate);
        public decimal? CalculateNetFromVat(decimal? vatAmount, in decimal vatRate);
    }
}
