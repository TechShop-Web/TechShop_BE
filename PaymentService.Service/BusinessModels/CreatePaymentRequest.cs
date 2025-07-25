using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Service.BusinessModels
{
    public class CreatePaymentRequest
    {
        public int OrderId { get; set; }
        public string PaymentMethod { get; set; }
        public float Amount { get; set; }
    }
}
