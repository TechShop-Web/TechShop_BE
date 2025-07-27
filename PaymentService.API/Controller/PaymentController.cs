using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Service.BusinessModels;
using PaymentService.Service.Interfaces;

namespace PaymentService.API.Controller
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentController(IPaymentService service)
        {
            _service = service;
        }

        [HttpPost("create-vnpay")]
        public async Task<IActionResult> CreateVNPay([FromBody] CreatePaymentRequest request)
        {
            var paymentUrl = _service.CreateVNPayPaymentUrlAsync(request, HttpContext);
            return Ok(new { url = paymentUrl });
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetById(int id)
        //{
        //    var result = await _service.GetByIdAsync(id);
        //    return result == null ? NotFound() : Ok(result);
        //}

        //[HttpGet("by-order/{orderId}")]
        //public async Task<IActionResult> GetByOrderId(int orderId)
        //{
        //    var result = await _service.GetByOrderIdAsync(orderId);
        //    return result == null ? NotFound() : Ok(result);
        //}

        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VNPayReturn()
        {
            var success = await _service.PaymentExecuteAsync(Request.Query);
            return Redirect("https://localhost:7262/api/payments/vnpay-return");
        }
    }

}
