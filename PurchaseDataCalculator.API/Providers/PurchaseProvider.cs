using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PurchaseDataCalculatorAPI.Interfaces;
using PurchaseDataCalculatorAPI.Models;

namespace PurchaseDataCalculatorAPI.Providers
{
    public class PurchaseProvider : IPurchaseProvider
    {
        private readonly ILogger<PurchaseProvider> _logger;
        private readonly IGrossCalculator _grossCalculator;
        private readonly IVatCalculator _vatCalculator;
        private readonly INetCalculator _netCalculator;

        public PurchaseProvider(ILogger<PurchaseProvider> logger, IGrossCalculator grossCalculator, IVatCalculator vatCalculator, INetCalculator netCalculator)
        {
            _logger = logger;
            _grossCalculator = grossCalculator;
            _vatCalculator = vatCalculator;
            _netCalculator = netCalculator;
        }

        public Task<(bool IsSuccess, Purchase Purchase, string ErrorMessage)> GetPurchaseVatAsync(Purchase purchase)
        {
            try
            {
                _logger?.LogInformation("Calculating Purchase Data");

                Validate(purchase);

                PurchaseBase purchaseBase = null;

                //Simple factory for purchase calculation
                switch (purchase)
                {
                    case { } p when p.GrossAmount != null && p.GrossAmount != 0:
                        purchaseBase = new PurchaseCalculatorWithGross(purchase.VatRate, purchase.GrossAmount, _netCalculator, _vatCalculator);
                        break;
                    case { } p when p.VatAmount != null && p.VatAmount != 0:
                        purchaseBase = new PurchaseCalculatorWithVat(purchase.VatRate, purchase.VatAmount, _netCalculator, _grossCalculator);
                        break;
                    case { } p when p.NetAmount != null && p.NetAmount != 0:
                        purchaseBase = new PurchaseCalculatorWithNet(purchase.VatRate, purchase.NetAmount, _grossCalculator, _vatCalculator);
                        break;
                }

                var result = purchaseBase.Calculate();

                _logger?.LogInformation("Purchase Data calculated");

                return Task.FromResult<(bool, Purchase, string)>((true, result, null));
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                return Task.FromResult<(bool, Purchase, string)>((false, null, ex.Message));
            }

        }

        private void Validate(Purchase purchase)
        {
            var errors = new StringBuilder();

            //Valid VAT rate values are : 10, 13, 20
            if (purchase.VatRate != 10 && purchase.VatRate != 13 && purchase.VatRate != 20)
                errors.Append("Please provide a valid VAT rate. Valid VAT rate values are : 10, 13, 20. Mandatory field.");

            if ((purchase.GrossAmount == null && purchase.NetAmount == null && purchase.VatAmount == null)
                    || (purchase.GrossAmount == 0 || purchase.NetAmount == 0 || purchase.VatAmount == 0))
                errors.Append("Please provide at least one not 0 amount value (Gross, Net or VAT amount)");

            if ((purchase.GrossAmount > 0 && (purchase.NetAmount != null || purchase.VatAmount != null))
                    || (purchase.NetAmount > 0 && (purchase.GrossAmount != null || purchase.VatAmount != null))
                    || (purchase.VatAmount > 0 && (purchase.NetAmount != null || purchase.GrossAmount != null)))
                errors.Append("Please provide only one amount (Gross, Net or VAT amount).");

            if (errors.Length > 0)
                throw new ValidationException(errors.ToString());
        }
    }
}