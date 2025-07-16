using Microsoft.AspNetCore.Mvc;
using ProductService.Repository.Models;
using ProductService.Service.Services;

namespace ProductService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/product
        [HttpGet("list")]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        // GET: api/product/5
        [HttpGet("detail/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // POST: api/product
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Product product)
        {
            await _productService.AddProductAsync(product);
            return Ok(product); // hoặc CreatedAtAction nếu muốn trả location
        }

        // PUT: api/product/5
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Product product)
        {
            if (id != product.Id)
                return BadRequest();

            await _productService.UpdateProductAsync(product);
            return NoContent();
        }

        // DELETE: api/product/5
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }
    }
}
