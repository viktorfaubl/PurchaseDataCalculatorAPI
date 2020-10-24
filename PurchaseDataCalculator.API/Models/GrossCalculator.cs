using System;
using PurchaseDataCalculatorAPI.Interfaces;

namespace PurchaseDataCalculatorAPI.Models
{
    public class GrossCalculator : IGrossCalculator
    {
        public decimal CalculateGross(decimal? netAmount, decimal vatRate)
        {
            return Math.Round(netAmount.Value * (1 + vatRate / 100) , 2);
        }
    }
}
