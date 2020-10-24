using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using PurchaseDataCalculatorAPI.Interfaces;
using PurchaseDataCalculatorAPI.Models;
using PurchaseDataCalculatorAPI.Providers;
using Xunit;

namespace PurchaseDataCalculator.API.UnitTest
{
    public class PurchaseTest
    {
        /// <summary>
        /// Test for valid VatRate amounts
        /// </summary>
        /// <param name="vatRate"></param>
        [Theory,
        InlineData(10),
        InlineData(13),
        InlineData(20)]
        public async void VatRateValidation(decimal vatRate)
        {
            // Arrange
            var serviceProvider = ServiceProvider();
            var purchaseProvider = new PurchaseProvider(null, serviceProvider);


            //Act
            var purchase = await purchaseProvider.GetPurchaseVatAsync(
                new Purchase {GrossAmount = 120,NetAmount = null,VatAmount = null, VatRate = vatRate});

            Assert.True(purchase.IsSuccess);
            Assert.NotNull(purchase.Purchase);
            Assert.Null(purchase.ErrorMessage);
        }

        private static ServiceProvider ServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddScoped<IGrossCalculator, GrossCalculator>();
            services.AddScoped<INetCalculator, NetCalculator>();
            services.AddScoped<IVatCalculator, VatCalculator>();
            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }

        /// <summary>
        /// Testing invalid VatRate numbers
        /// </summary>
        /// <param name="vatRate"></param>
        [Theory,
         InlineData(18),
         InlineData(19),
         InlineData(26)]
        public async void VatRateInvalidValidation(decimal vatRate)
        {
            // Arrange
            var serviceProvider = ServiceProvider();
            var purchaseProvider = new PurchaseProvider(null, serviceProvider);

            // Act
            var purchase = await purchaseProvider.GetPurchaseVatAsync(
                new Purchase { GrossAmount = 120, NetAmount = null, VatAmount = null, VatRate = vatRate });

            // Assert
            Assert.False(purchase.IsSuccess);
            Assert.Null(purchase.Purchase);
            Assert.NotNull(purchase.ErrorMessage);
        }

        /// <summary>
        /// Testing data providing Gross 
        /// </summary>
        /// <param name="vatRate"></param>
        /// <param name="grossAmount"></param>
        /// <param name="netAmount"></param>
        /// <param name="vatAmount"></param>
        [Theory,
         InlineData(10, 120, 109.09, 10.91),
         InlineData(13, 300, 265.49, 34.51),
         InlineData(20, 100, 83.33, 16.67)]
        public async void CalculateAmountFromGross(decimal vatRate, decimal grossAmount, decimal netAmount, decimal vatAmount)
        {
            // Arrange
            var serviceProvider = ServiceProvider();
            var purchaseProvider = new PurchaseProvider(null, serviceProvider);

            // Act
            var purchase = await purchaseProvider.GetPurchaseVatAsync(
                new Purchase { GrossAmount = grossAmount, NetAmount = null, VatAmount = null, VatRate = vatRate });

            // Assert
            Assert.True(purchase.IsSuccess);
            Assert.NotNull(purchase.Purchase);
            Assert.Null(purchase.ErrorMessage);
            Assert.Equal(purchase.Purchase.NetAmount, netAmount);
            Assert.Equal(purchase.Purchase.VatAmount, vatAmount);
        }

        /// <summary>
        /// Testing data providing Net Amount
        /// </summary>
        /// <param name="vatRate"></param>
        /// <param name="grossAmount"></param>
        /// <param name="netAmount"></param>
        /// <param name="vatAmount"></param>
        [Theory,
         InlineData(10, 120, 109.09, 10.91),
         InlineData(13, 300, 265.49, 34.51),
         InlineData(20, 100, 83.33, 16.67)]
        public async void CalculateAmountFromNet(decimal vatRate, decimal grossAmount, decimal netAmount, decimal vatAmount)
        {
            // Arrange
            var serviceProvider = ServiceProvider();
            var purchaseProvider = new PurchaseProvider(null, serviceProvider);

            // Act
            var purchase = await purchaseProvider.GetPurchaseVatAsync(
                new Purchase { GrossAmount = null, NetAmount = netAmount, VatAmount = null, VatRate = vatRate });

            // Assert
            Assert.True(purchase.IsSuccess);
            Assert.NotNull(purchase.Purchase);
            Assert.Null(purchase.ErrorMessage);
            Assert.Equal(purchase.Purchase.GrossAmount, grossAmount);
            Assert.Equal(purchase.Purchase.VatAmount, vatAmount);
        }

        /// <summary>
        /// Testing data providing VatAmount
        /// </summary>
        /// <param name="vatRate"></param>
        /// <param name="grossAmount"></param>
        /// <param name="netAmount"></param>
        /// <param name="vatAmount"></param>
        [Theory,
         InlineData(10, 120.01, 109.1, 10.91),
         InlineData(13, 299.97, 265.46, 34.51),
         InlineData(20, 100.02, 83.35, 16.67)]
        public async void CalculateAmountFromVat(decimal vatRate, decimal grossAmount, decimal netAmount, decimal vatAmount)
        {
            /*
             Amount are correct according to the https://www.calkoo.com/en/vat-calculator, however there could be a rounding problem
            Further clarification should needed to see this data is correct or not
             */

            // Arrange
            var serviceProvider = ServiceProvider();
            var purchaseProvider = new PurchaseProvider(null, serviceProvider);

            // Act
            var purchase = await purchaseProvider.GetPurchaseVatAsync(
                new Purchase { GrossAmount = null, NetAmount = null, VatAmount = vatAmount, VatRate = vatRate });

            // Assert
            Assert.True(purchase.IsSuccess);
            Assert.NotNull(purchase.Purchase);
            Assert.Null(purchase.ErrorMessage);
            Assert.Equal(purchase.Purchase.GrossAmount, grossAmount);
            Assert.Equal(purchase.Purchase.NetAmount, netAmount);
        }

        /// <summary>
        /// Testing data providing Gross with Invalid data
        /// </summary>
        /// <param name="vatRate"></param>
        /// <param name="grossAmount"></param>
        /// <param name="netAmount"></param>
        /// <param name="vatAmount"></param>
        [Theory,
         InlineData(10, 130, 109.09, 10.91),
         InlineData(13, 320, 265.49, 34.51),
         InlineData(20, 120, 83.33, 16.67)]
        public async void CalculateAmountFromGrossInvalid(decimal vatRate, decimal grossAmount, decimal netAmount, decimal vatAmount)
        {
            // Arrange
            var serviceProvider = ServiceProvider();
            var purchaseProvider = new PurchaseProvider(null, serviceProvider);

            // Act
            var purchase = await purchaseProvider.GetPurchaseVatAsync(
                new Purchase { GrossAmount = grossAmount, NetAmount = null, VatAmount = null, VatRate = vatRate });

            // Assert
            Assert.True(purchase.IsSuccess);
            Assert.NotNull(purchase.Purchase);
            Assert.Null(purchase.ErrorMessage);
            Assert.NotEqual(purchase.Purchase.NetAmount, netAmount);
            Assert.NotEqual(purchase.Purchase.VatAmount, vatAmount);
        }

        /// <summary>
        /// Testing data providing Net Amount with Invalid data
        /// </summary>
        /// <param name="vatRate"></param>
        /// <param name="grossAmount"></param>
        /// <param name="netAmount"></param>
        /// <param name="vatAmount"></param>
        [Theory,
         InlineData(10, 140, 109.09, 20),
         InlineData(13, 320, 265.49, 80),
         InlineData(20, 120, 83.33, 89)]
        public async void CalculateAmountFromNetInvalid(decimal vatRate, decimal grossAmount, decimal netAmount, decimal vatAmount)
        {
            // Arrange
            var serviceProvider = ServiceProvider();
            var purchaseProvider = new PurchaseProvider(null, serviceProvider);

            // Act
            var purchase = await purchaseProvider.GetPurchaseVatAsync(
                new Purchase { GrossAmount = null, NetAmount = netAmount, VatAmount = null, VatRate = vatRate });

            // Assert
            Assert.True(purchase.IsSuccess);
            Assert.NotNull(purchase.Purchase);
            Assert.Null(purchase.ErrorMessage);
            Assert.NotEqual(purchase.Purchase.GrossAmount, grossAmount);
            Assert.NotEqual(purchase.Purchase.VatAmount, vatAmount);
        }

        /// <summary>
        /// Testing data providing VatAmount with Invalid data
        /// </summary>
        /// <param name="vatRate"></param>
        /// <param name="grossAmount"></param>
        /// <param name="netAmount"></param>
        /// <param name="vatAmount"></param>
        [Theory,
         InlineData(10, 130, 109.09, 10.91),
         InlineData(13, 320, 265.49, 34.51),
         InlineData(20, 120, 83.33, 16.67)]
        public async void CalculateAmountFromVatInvalid(decimal vatRate, decimal grossAmount, decimal netAmount, decimal vatAmount)
        {
            // Arrange
            var serviceProvider = ServiceProvider();
            var purchaseProvider = new PurchaseProvider(null, serviceProvider);

            // Act
            var purchase = await purchaseProvider.GetPurchaseVatAsync(
                new Purchase { GrossAmount = null, NetAmount = null, VatAmount = vatAmount, VatRate = vatRate });

            // Assert
            Assert.True(purchase.IsSuccess);
            Assert.NotNull(purchase.Purchase);
            Assert.Null(purchase.ErrorMessage);
            Assert.NotEqual(purchase.Purchase.GrossAmount, grossAmount);
            Assert.NotEqual(purchase.Purchase.NetAmount, netAmount);
        }
    }
}
