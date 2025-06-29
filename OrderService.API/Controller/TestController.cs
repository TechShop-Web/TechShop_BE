using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using ProductService;

namespace OrderService.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ProductService.ProductService.ProductServiceClient _productClient;

        public TestController(ProductService.ProductService.ProductServiceClient productClient)
        {
            _productClient = productClient ?? throw new ArgumentNullException(nameof(productClient));
        }

        [HttpPost("check-variant")]
        public async Task<IActionResult> CheckVariantAvailability([FromBody] VariantRequestDto request)
        {
            if (request == null)
            {
                return BadRequest("Request body is required.");
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
                return Ok(new
                {
                    Success = true,
                    ProductExists = variantResponse.ProductExists,
                    VariantExists = variantResponse.VariantExists,
                    StockQuantity = variantResponse.StockQuantity,
                    Error = string.IsNullOrEmpty(variantResponse.Error) ? "None" : variantResponse.Error
                });
            }
            catch (RpcException ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Error = $"gRPC Error: {ex.Status.Detail}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Error = $"Unexpected Error: {ex.Message}"
                });
            }
        }
    }

    public class VariantRequestDto
    {
        public int ProductId { get; set; }
        public int VariantId { get; set; }
        public int Quantity { get; set; }
    }
}
