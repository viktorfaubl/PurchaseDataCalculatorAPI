using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PurchaseDataCalculatorAPI.Interfaces;
using PurchaseDataCalculatorAPI.Models;

namespace PurchaseDataCalculatorAPI.Controllers
{
    [ApiController]
    [Route("api/purchase")]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseProvider _purchaseProvider;

        public PurchaseController(IPurchaseProvider purchaseProvider)
        {
            _purchaseProvider = purchaseProvider;
        }
        
        /// <summary>
        /// GET method according to RestFull API architectural style
        /// </summary>
        /// <param name="purchase">Query string</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetPurchaseVatAsync([FromQuery]Purchase purchase)
        {

            var result = await _purchaseProvider.GetPurchaseVatAsync(purchase);
            if (result.IsSuccess)
            {
                return Ok(result.Purchase);
            }
            return BadRequest(result.ErrorMessage);
        }
        
    }
}