using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Service.Interfaces;
using OrderService.Service.Models;
using ProductService;
using System.Security.Claims;

namespace OrderService.API.Controller
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ProductService.ProductService.ProductServiceClient _productClient;
        public OrderController(IOrderService orderService, ProductService.ProductService.ProductServiceClient productClient)
        {
            _orderService = orderService;
            _productClient = productClient ?? throw new ArgumentNullException(nameof(productClient));
        }

        // POST: api/Order
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
        {
            if (request == null || request.OrderItems == null || request.OrderItems.Count == 0)
            {
                throw new ArgumentException("Order request and order items cannot be null or empty.", nameof(request));
            }

            foreach (var item in request.OrderItems)
            {
                var variantRequest = new VariantRequest
                {
                    ProductId = item.ProductId,
                    VariantId = item.VariantId,
                    Quantity = item.Quantity
                };
                try
                {
                    var variantResponse = await _productClient.CheckVariantAvailabilityAsync(variantRequest);
                    if (!variantResponse.VariantExists || variantResponse.StockQuantity < item.Quantity)
                    {
                        return BadRequest($"Variant {item.VariantName} for product {item.ProductName} is not available in the requested quantity.");
                    }
                }
                catch (RpcException ex)
                {
                    return StatusCode(500, $"gRPC Error: {ex.Status.Detail}");
                }
            }

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
                {
                    return BadRequest("Invalid user ID.");
                }
                var result = await _orderService.CreateOrderAsync(parsedUserId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Order/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            return Ok();
        }

        // GET: api/Order/user/{userId}
        [HttpGet("user/{userId:int}")]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            return Ok();
        }

        // PUT: api/Order/{id}/cancel
        [HttpPut("{id:int}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            return NoContent();
        }
    }
}
