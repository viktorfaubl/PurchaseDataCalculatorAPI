using System.Threading.Tasks;
using PurchaseDataCalculatorAPI.Models;

namespace PurchaseDataCalculatorAPI.Interfaces
{
    public interface IPurchaseProvider
    {
        Task<(bool IsSuccess, Purchase Purchase, string ErrorMessage)> GetPurchaseVatAsync(Purchase purchase);
    }
}
