using CartService.Repository.Models;
using CartService.Service.Interfaces;
using CartService.Service.Models;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService;
using System.Security.Claims;

namespace CartService.API.Controller
{
    [Route("api/Carts")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _CartService;
        private readonly ProductService.ProductService.ProductServiceClient _productClient;
        private readonly UserService.UserService.UserServiceClient _userClient;

        public CartController(ICartService CartService, ProductService.ProductService.ProductServiceClient productClient, UserService.UserService.UserServiceClient userClient)
        {
            _CartService = CartService;
            _productClient = productClient ?? throw new ArgumentNullException(nameof(productClient));
            _userClient = userClient;
        }

        // POST: api/Carts
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateCart([FromBody] CartRequest request)
        {
            if (request == null || !request.VariantId.HasValue || !request.Quantity.HasValue || !request.UnitPrice.HasValue)
                return BadRequest("Invalid cart request.");

            var variantRequest = new VariantRequest
            {
                //ProductId = 0, // Replace with actual ProductId logic if needed
                VariantId = request.VariantId.Value,
                Quantity = request.Quantity.Value
            };

            try
            {
                var variantResponse = await _productClient.CheckVariantAvailabilityAsync(variantRequest);
                if (!variantResponse.VariantExists || variantResponse.StockQuantity < request.Quantity.Value)
                    return BadRequest($"Variant is not available in the requested quantity.");
            }
            catch (RpcException ex)
            {
                return StatusCode(500, $"gRPC Error: {ex.Status.Detail}");
            }

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
                    return BadRequest("Invalid user ID.");

                var cartItem = new CartItem
                {
                    UserId = parsedUserId,
                    VariantId = request.VariantId,
                    Quantity = request.Quantity,
                    UnitPrice = request.UnitPrice,
                    CreatedAt = DateTime.UtcNow
                };
                var result = await _CartService.CreateCartAsync(parsedUserId, cartItem);
                return Ok(result.Data); // Return CartItem
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Carts/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCartById(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid Cart ID.");
            var result = await _CartService.GetCartByIdAsync(id);
            return result.Success ? Ok(result.Data) : NotFound(result.Message);
        }

        // GET: api/Carts/user/{userId}
        [HttpGet("user/{userId:int}")]
        public async Task<IActionResult> GetCartsByUserId(int userId)
        {
            if (userId <= 0)
                return BadRequest("Invalid user ID.");
            var result = await _CartService.GetCartsByUserIdAsync(userId);
            return result.Success ? Ok(result.Data) : NotFound(result.Message);
        }

        // PATCH: api/Carts/{id}/cancel
        [HttpPatch("{id:int}/cancel")]
        public IActionResult CancelCart(int id)
        {
            return Ok("This endpoint is not implemented yet. Please check back later.");
        }

        [HttpGet("test/{id}")]
        public async Task<IActionResult> TestUserService(int id)
        {
            try
            {
                var userResponse = await _userClient.CheckUserExistsAsync(new UserService.UserCheckRequest { UserId = id });
                return Ok(userResponse);
            }
            catch (RpcException ex)
            {
                return StatusCode(500, $"gRPC Error: {ex.Status.Detail}");
            }
        }
    }
}