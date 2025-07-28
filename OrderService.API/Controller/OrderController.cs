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
        private readonly UserService.UserService.UserServiceClient _userClient;
        private readonly IOrderStatisticsService _statisticsService;

        public OrderController(IOrderService orderService, ProductService.ProductService.ProductServiceClient productClient, UserService.UserService.UserServiceClient userClient, IOrderStatisticsService statisticsService)
        {
            _orderService = orderService;
            _productClient = productClient ?? throw new ArgumentNullException(nameof(productClient));
            _userClient = userClient;
            _statisticsService = statisticsService ?? throw new ArgumentNullException(nameof(statisticsService));
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

                    if (!variantResponse.ProductExists)
                    {
                        return BadRequest($"Product {item.ProductName} does not exist.");
                    }

                    if (!variantResponse.VariantExists)
                    {
                        return BadRequest($"Variant {item.VariantName} for product {item.ProductName} does not exist.");
                    }

                    if (variantResponse.StockQuantity <= 0)
                    {
                        return BadRequest($"Variant {item.VariantName} for product {item.ProductName} is out of stock.");
                    }

                    if (variantResponse.StockQuantity < item.Quantity)
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
            if (id <= 0)
            {
                return BadRequest("Invalid order ID.");
            }
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }
            return Ok(order);
        }

        // GET: api/Order/user/{userId}
        [HttpGet("user/{userId:int}")]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }
            var order = await _orderService.GetOrdersByUserIdAsync(userId);
            if (order == null)
            {
                return NotFound($"Order with user ID {userId} not found.");
            }
            return Ok(order);
        }

        // PUT: api/Order/{id}/cancel
        [HttpPatch("{id:int}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                var result = await _orderService.CancelOrderAsync(id, "User requested cancellation.");
                if (!result.Success)
                {
                    return BadRequest(result.Message);
                }
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        // PUT: api/Order/{id}/status
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest("Status cannot be null or empty.");
            }
            try
            {
                var result = await _orderService.UpdateOrderStatusAsync(id, status);
                if (!result.Success)
                {
                    return BadRequest(result.Message);
                }
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("test/{id}")]
        public async Task<IActionResult> TestUserService(int id)
        {
            try
            {
                var userResponse = await _userClient.CheckUserExistsAsync(new UserService.UserCheckRequest
                {
                    UserId = id
                });
                return Ok(userResponse);
            }
            catch (RpcException ex)
            {
                return StatusCode(500, $"gRPC Error: {ex.Status.Detail}");
            }

        }
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _statisticsService.GetOrderStatisticsAsync(startDate, endDate, pageIndex, pageSize);
            return Ok(result);
        }
    }
}
