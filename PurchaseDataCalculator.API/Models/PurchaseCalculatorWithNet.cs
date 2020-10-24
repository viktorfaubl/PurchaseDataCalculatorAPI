using System;
using PurchaseDataCalculatorAPI.Interfaces;

namespace PurchaseDataCalculatorAPI.Models
{
    public class PurchaseCalculatorWithNet : PurchaseBase
    {
        private readonly IGrossCalculator _grossCalculator;
        private readonly IVatCalculator _vatCalculator;

        public PurchaseCalculatorWithNet(decimal vatRate, decimal? netAmount, IServiceProvider serviceProvider)
        {
            _grossCalculator = (IGrossCalculator)serviceProvider.GetService(typeof(IGrossCalculator));
            _vatCalculator = (IVatCalculator)serviceProvider.GetService(typeof(IVatCalculator));
            VatRate = vatRate;
            NetAmount = netAmount;
        }

        public override Purchase Calculate()
        {
            GrossAmount = _grossCalculator.CalculateGross(NetAmount, VatRate);
            VatAmount = _vatCalculator.CalculateVat(GrossAmount, VatRate);
            
            //TODO automapper
            return new Purchase { GrossAmount = GrossAmount, NetAmount = NetAmount, VatAmount = VatAmount, VatRate = VatRate };
        }

    }
}
