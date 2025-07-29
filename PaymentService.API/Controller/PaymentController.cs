using Azure;
using Azure.Core;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using OrderService.Grpc;
using PaymentService.Service.BusinessModels;
using PaymentService.Service.Interfaces;
using static OrderService.Grpc.OrderService;

namespace PaymentService.API.Controller
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;
        private readonly OrderServiceClient _orderServiceClient;
        private readonly ILogger<PaymentController> _logger;



        public PaymentController(IPaymentService service, OrderServiceClient orderServiceClient, ILogger<PaymentController> logger)
        {
            _service = service;
            _orderServiceClient = orderServiceClient;
            _logger=logger;
        }

        [HttpPost("create-vnpay")]
        public async Task<IActionResult> CreateVNPay([FromBody] CreatePaymentRequest request)
        {
            try
            {
                var orderRequest = new GetOrderRequest { OrderId = request.OrderId };
                var orderResponse = await _orderServiceClient.GetOrderAsync(orderRequest);

                if (orderResponse == null)
                {
                    return BadRequest($"Order {request.OrderId} not found");
                }

                var updatedRequest = new CreatePaymentRequest
                {
                    OrderId = orderResponse.Id, 
                    PaymentMethod = request.PaymentMethod,
                    Amount = (float)orderResponse.TotalAmount
                };

                var paymentUrl = await _service.CreateVNPayPaymentUrlAsync(updatedRequest, HttpContext);
                return Ok(new { url = paymentUrl });
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error when getting order {OrderId}: {Status}", request.OrderId, ex.Status);
                return BadRequest($"Failed to get order details: {ex.Status.Detail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating VNPay payment for order {OrderId}", request.OrderId);
                return StatusCode(500, "Internal server error");
            }
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
