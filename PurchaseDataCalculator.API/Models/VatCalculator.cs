using System;
using PurchaseDataCalculatorAPI.Interfaces;

namespace PurchaseDataCalculatorAPI.Models
{
    public class VatCalculator : IVatCalculator
    {
        public decimal CalculateVat(decimal? grossAmount, decimal vatRate)
        {
            return Math.Round(grossAmount.Value  - grossAmount.Value  / (1 + vatRate / 100), 2);
        }
    }
}
