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
            return result == null ? NotFound() : Ok(result);
        }

        // POST: api/productvariant/create
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ProductVariant variant)
        {
            var created = await _variantService.CreateAsync(variant);
            return Ok(created);
        }

        // PUT: api/productvariant/update/{id}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductVariant variant)
        {
            if (id != variant.Id) return BadRequest();
            var success = await _variantService.UpdateAsync(variant);
            return success ? NoContent() : NotFound();
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
