using CartService.Repository.Models;
using CartService.Service.Interfaces;
using CartService.Service.Models;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CartService.API.Controller
{
    [Route("api/Carts")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ProductService.ProductService.ProductServiceClient _productClient;
        private readonly UserService.UserService.UserServiceClient _userClient;
        private readonly ILogger<CartController> _logger;

        public CartController(
            ICartService cartService,
            ProductService.ProductService.ProductServiceClient productClient,
            UserService.UserService.UserServiceClient userClient,
            ILogger<CartController> logger)
        {
            _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            _productClient = productClient ?? throw new ArgumentNullException(nameof(productClient));
            _userClient = userClient ?? throw new ArgumentNullException(nameof(userClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // POST: api/Carts
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateCart([FromBody] CartRequest request)
        {
            if (request == null)
            {
                _logger.LogWarning("Cart request is null.");
                return BadRequest("Cart request cannot be null.");
            }

            var variantRequest = new VariantRequest
            {
                ProductId = request.ProductId,
                VariantId = request.VariantId,
                Quantity = request.Quantity
            };

            try
            {
                var variantResponse = await _productClient.CheckVariantAvailabilityAsync(variantRequest);
                if (!variantResponse.VariantExists || variantResponse.StockQuantity < request.Quantity)
                {
                    _logger.LogWarning("Variant not available: ProductId={ProductId}, VariantId={VariantId}, RequestedQuantity={Quantity}, Available={Stock}",
                        request.ProductId, request.VariantId, request.Quantity, variantResponse.StockQuantity);
                    return BadRequest("Variant is not available in the requested quantity.");
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
                {
                    _logger.LogWarning("Invalid user ID from claims.");
                    return BadRequest("Invalid user ID.");
                }

                var userResponse = await _userClient.CheckUserExistsAsync(new UserService.UserCheckRequest { UserId = parsedUserId });
                if (!userResponse.Exists)
                {
                    _logger.LogWarning("User not found: UserId={UserId}", parsedUserId);
                    return BadRequest("User not found.");
                }

                var cartItem = new CartItem
                {
                    UserId = parsedUserId,
                    VariantId = request.VariantId,
                    Quantity = request.Quantity,
                    UnitPrice = request.UnitPrice,
                    CreatedAt = DateTime.UtcNow
                };
                var result = await _cartService.CreateCartAsync(parsedUserId, cartItem);
                _logger.LogInformation("Cart created successfully: CartId={CartId}, UserId={UserId}", cartItem.Id, parsedUserId);
                return Ok(result.Data);
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error while checking variant or user: {Message}", ex.Status.Detail);
                return StatusCode(500, $"gRPC Error: {ex.Status.Detail}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Carts/{id}
        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCartById(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid Cart ID: {Id}", id);
                return BadRequest("Invalid Cart ID.");
            }

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
                {
                    _logger.LogWarning("Invalid user ID from claims.");
                    return BadRequest("Invalid user ID.");
                }

                var result = await _cartService.GetCartByIdAsync(id);
                if (!result.Success)
                {
                    _logger.LogWarning("Cart not found: Id={Id}", id);
                    return NotFound(result.Message);
                }

                if (result.Data.UserId != parsedUserId)
                {
                    _logger.LogWarning("Unauthorized access to cart: CartId={CartId}, UserId={UserId}", id, parsedUserId);
                    return Unauthorized("You are not authorized to view this cart.");
                }

                _logger.LogInformation("Cart retrieved successfully: CartId={CartId}, UserId={UserId}", id, parsedUserId);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart: CartId={CartId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

      

   

        // DELETE: api/Carts/{id}
        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid Cart ID: {Id}", id);
                return BadRequest("Invalid Cart ID.");
            }

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
                {
                    _logger.LogWarning("Invalid user ID from claims.");
                    return BadRequest("Invalid user ID.");
                }

                var result = await _cartService.DeleteCartAsync(parsedUserId, id);
                if (!result.Success)
                {
                    _logger.LogWarning("Cart not found or unauthorized: CartId={CartId}, UserId={UserId}", id, parsedUserId);
                    return NotFound(result.Message);
                }

                _logger.LogInformation("Cart deleted successfully: CartId={CartId}, UserId={UserId}", id, parsedUserId);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cart: CartId={CartId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllCart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID cannot be null or empty.");
                return BadRequest("User ID cannot be null or empty.");
            }
            try
            {
                var result = await _cartService.GetAllCartAsync(userId);
                if (!result.Success || result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning("No carts found for user: UserId={UserId}", userId);
                    return NotFound(result.Message);
                }
                _logger.LogInformation("Carts retrieved successfully for user: UserId={UserId}", userId);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving carts for user: UserId={UserId}", userId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}