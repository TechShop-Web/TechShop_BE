using Grpc.Core;
using ProductService.Service.Services;

namespace ProductService.API.Services.Grpc
{
    public class ProductGrpcService : ProductService.ProductServiceBase
    {
        private readonly IProductService _productService;
        private readonly IProductVariantService _variantService;

        public ProductGrpcService(IProductService productService, IProductVariantService variantService)
        {
            _productService = productService;
            _variantService = variantService;
        }

        public override async Task<VariantResponse> CheckVariantAvailability(VariantRequest request, ServerCallContext context)
        {
            if (request.ProductId <= 0 || request.VariantId <= 0)
            {
                return new VariantResponse { Error = "Invalid ProductId or VariantId" };
            }

            var product = await _productService.GetProductByIdAsync(request.ProductId);
            var variant = await _variantService.GetByIdAsync(request.VariantId);

            if (product == null || variant == null || variant.ProductId != request.ProductId)
            {
                return new VariantResponse
                {
                    ProductExists = product != null,
                    VariantExists = false,
                    StockQuantity = 0,
                    Error = "Product or Variant not found"
                };
            }

            var stockQuantity = variant.Stock;

            return new VariantResponse
            {
                ProductExists = true,
                VariantExists = true,
                StockQuantity = stockQuantity,
                Error = ""
            };
        }
    }
}
