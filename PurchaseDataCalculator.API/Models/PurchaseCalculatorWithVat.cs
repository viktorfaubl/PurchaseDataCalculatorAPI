using System;
using Microsoft.Extensions.DependencyInjection;
using PurchaseDataCalculatorAPI.Interfaces;

namespace PurchaseDataCalculatorAPI.Models
{
    public class PurchaseCalculatorWithVat : PurchaseBase
    {
        private readonly INetCalculator _netCalculator;
        private readonly IGrossCalculator _grossCalculator;

        public PurchaseCalculatorWithVat(decimal vatRate, decimal? vatAmount, IServiceProvider serviceProvider)
        {
            _netCalculator = serviceProvider.GetService<INetCalculator>();
            _grossCalculator = serviceProvider.GetService<IGrossCalculator>();
            VatRate = vatRate;
            VatAmount = vatAmount;
        }

        public override Purchase Calculate()
        {
            NetAmount = _netCalculator.CalculateNetFromVat(VatAmount, VatRate);
            GrossAmount = _grossCalculator.CalculateGross(NetAmount, VatRate);

            //TODO automapper
            return new Purchase { GrossAmount = GrossAmount, NetAmount = NetAmount, VatAmount = VatAmount, VatRate = VatRate };
        }

    }
}
