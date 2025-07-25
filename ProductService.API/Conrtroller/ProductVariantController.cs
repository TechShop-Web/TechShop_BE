using Microsoft.AspNetCore.Mvc;
using ProductService.Repository.Models;
using ProductService.Service.Services;

namespace ProductService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductVariantController : ControllerBase
    {
        private readonly IProductVariantService _variantService;

        public ProductVariantController(IProductVariantService variantService)
        {
            _variantService = variantService;
        }

        // GET: api/productvariant/list
        [HttpGet("list")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _variantService.GetAllAsync();
            return Ok(result);
        }

        // GET: api/productvariant/detail/{id}
        [HttpGet("detail/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _variantService.GetByIdAsync(id);
            if (result == null)
            {
                // Trả về thông điệp khi không tìm thấy sản phẩm variant
                var notFoundResponse = new
                {
                    message = "Data not found",
                    data = new List<ProductVariant>() // Hoặc bạn có thể trả về giá trị mặc định khác
                };
                return NotFound(notFoundResponse);
            }
            return Ok(result);
        }


        // GET: api/productvariant/groupwithproduct/{productId}
        [HttpGet("groupwithproduct/{productId}")]
        public async Task<IActionResult> GetVariantsByProductIdWithProductName(int productId)
        {
            var result = await _variantService.GetVariantsByProductIdWithProductNameAsync(productId);

            if (result == null || !result.Any())
            {
                var notFoundResponse = new
                {
                    message = "No variants found for this product",
                    data = new List<object>()
                };
                return NotFound(notFoundResponse);
            }

            return Ok(result);
        }


        // POST: api/productvariant/create
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ProductVariant variant)
        {
            try
            {
                var createdVariant = await _variantService.CreateAsync(variant);
                return Ok(createdVariant);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred.", message = ex.Message });
            }
        }

        // PUT: api/productvariant/update/{id}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductVariant variant)
        {
            if (id != variant.Id) return BadRequest(new { error = "ID mismatch." });

            try
            {
                var success = await _variantService.UpdateAsync(variant);
                if (success)
                    return NoContent(); 
                else
                    return NotFound(new { error = "No variant found to update" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred.", message = ex.Message });
            }
        }


        // DELETE: api/productvariant/delete/{id}
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _variantService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }


       


    }
}
