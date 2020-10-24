using System;
using PurchaseDataCalculatorAPI.Interfaces;

namespace PurchaseDataCalculatorAPI.Models
{
    public class PurchaseCalculatorWithGross : PurchaseBase
    {
        private readonly INetCalculator _netCalculator;
        private readonly IVatCalculator _vatCalculator;

        public PurchaseCalculatorWithGross(decimal vatRate, decimal? grossAmount, IServiceProvider serviceProvider)
        {
            _netCalculator = (INetCalculator)serviceProvider.GetService(typeof(INetCalculator));
            _vatCalculator = (IVatCalculator)serviceProvider.GetService(typeof(IVatCalculator)); ;
            VatRate = vatRate;
            GrossAmount = grossAmount;
        }

        public override Purchase Calculate()
        {
            NetAmount = _netCalculator.CalculateNet(GrossAmount, VatRate);
            VatAmount = _vatCalculator.CalculateVat(GrossAmount, VatRate);

            //TODO automapper
            return new Purchase {GrossAmount = GrossAmount,NetAmount = NetAmount, VatAmount = VatAmount, VatRate = VatRate};
        }

    }
}
