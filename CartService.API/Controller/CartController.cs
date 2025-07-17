//using Grpc.Core;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using CartService.Service.Interfaces;
//using CartService.Service.Models;
//using ProductService;
//using System.Security.Claims;

//namespace CartService.API.Controller
//{
//    [Route("api/Carts")]
//    [ApiController]
//    public class CartController : ControllerBase
//    {
//        private readonly ICartService _CartService;
//        private readonly ProductService.ProductService.ProductServiceClient _productClient;
//        private readonly UserService.UserService.UserServiceClient _userClient;
//        public CartController(ICartService CartService, ProductService.ProductService.ProductServiceClient productClient, UserService.UserService.UserServiceClient userClient)
//        {
//            _CartService = CartService;
//            _productClient = productClient ?? throw new ArgumentNullException(nameof(productClient));
//            _userClient = userClient;
//        }

//        // POST: api/Cart
//        [Authorize]
//        [HttpPost]
//        public async Task<IActionResult> CreateCart([FromBody] CartRequest request)
//        {
//            if (request == null || request.CartItems == null || request.CartItems.Count == 0)
//            {
//                throw new ArgumentException("Cart request and Cart items cannot be null or empty.", nameof(request));
//            }

//            foreach (var item in request.CartItems)
//            {
//                var variantRequest = new VariantRequest
//                {
//                    ProductId = item.ProductId,
//                    VariantId = item.VariantId,
//                    Quantity = item.Quantity
//                };
//                try
//                {
//                    var variantResponse = await _productClient.CheckVariantAvailabilityAsync(variantRequest);
//                    if (!variantResponse.VariantExists || variantResponse.StockQuantity < item.Quantity)
//                    {
//                        return BadRequest($"Variant {item.VariantName} for product {item.ProductName} is not available in the requested quantity.");
//                    }
//                }
//                catch (RpcException ex)
//                {
//                    return StatusCode(500, $"gRPC Error: {ex.Status.Detail}");
//                }
//            }

//            try
//            {
//                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
//                {
//                    return BadRequest("Invalid user ID.");
//                }
//                var result = await _CartService.CreateCartAsync(parsedUserId, request);
//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Internal server error: {ex.Message}");
//            }
//        }

//        // GET: api/Cart/{id}
//        [HttpGet("{id:int}")]
//        public async Task<IActionResult> GetCartById(int id)
//        {
//            if (id <= 0)
//            {
//                return BadRequest("Invalid Cart ID.");
//            }
//            var Cart = await _CartService.GetCartByIdAsync(id);
//            if (Cart == null)
//            {
//                return NotFound($"Cart with ID {id} not found.");
//            }
//            return Ok(Cart);
//        }

//        // GET: api/Cart/user/{userId}
//        [HttpGet("user/{userId:int}")]
//        public async Task<IActionResult> GetCartsByUserId(int userId)
//        {
//            if (userId <= 0)
//            {
//                return BadRequest("Invalid user ID.");
//            }
//            var Cart = await _CartService.GetCartByIdAsync(userId);
//            if (Cart == null)
//            {
//                return NotFound($"Cart with user ID {userId} not found.");
//            }
//            return Ok(Cart);
//        }

//        // PUT: api/Cart/{id}/cancel
//        [HttpPatch("{id:int}/cancel")]
//        public IActionResult CancelCart(int id)
//        {
//            return Ok("This endpoint is not implemented yet. Please check back later.");
//        }
//        [HttpGet("test/{id}")]
//        public async Task<IActionResult> TestUserService(int id)
//        {
//            try
//            {
//                var userResponse = await _userClient.CheckUserExistsAsync(new UserService.UserCheckRequest
//                {
//                    UserId = id
//                });
//                return Ok(userResponse);
//            }
//            catch (RpcException ex)
//            {
//                return StatusCode(500, $"gRPC Error: {ex.Status.Detail}");
//            }

//        }
//    }
//}
