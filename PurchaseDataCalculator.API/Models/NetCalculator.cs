using System;
using PurchaseDataCalculatorAPI.Interfaces;

namespace PurchaseDataCalculatorAPI.Models
{
    public class NetCalculator : INetCalculator
    {
        public decimal CalculateNet(decimal? grossAmount, decimal vatRate)
        {
            return Math.Round(grossAmount.Value / (1 + vatRate / 100), 2);
        }

        public decimal? CalculateNetFromVat(decimal? vatAmount, in decimal vatRate)
        {
            return Math.Round(vatAmount.Value / (vatRate / 100), 2);
        }
    }
}
