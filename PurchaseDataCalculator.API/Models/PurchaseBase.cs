using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PurchaseDataCalculatorAPI.Models
{
    public abstract class PurchaseBase
    {
        protected decimal VatRate;
        protected decimal? NetAmount;
        protected decimal? GrossAmount;
        protected decimal? VatAmount;
        public abstract Purchase Calculate();
    }
}
