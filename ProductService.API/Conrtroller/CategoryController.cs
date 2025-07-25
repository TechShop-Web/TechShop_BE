using Microsoft.AspNetCore.Mvc;
using ProductService.Repository.Models;
using ProductService.Service.Services;

namespace ProductService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/category/list
        [HttpGet("list")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _categoryService.GetAllAsync();
            return Ok(result);
        }

        // GET: api/category/detail/{id}
        //[HttpGet("detail/{id}")]
        //public async Task<IActionResult> Get(int id)
        //{
        //    var result = await _categoryService.GetByIdAsync(id);
        //    return result == null ? NotFound() : Ok(result);
        //}
        [HttpGet("detail/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            if (result == null)
            {
                var notFoundResponse = new
                {
                    message = "Data not found",
                    data = new List<Category>() // Hoặc một giá trị mặc định khác
                };
                return NotFound(notFoundResponse);
            }
            return Ok(result);
        }


        // POST: api/category/create
        //[HttpPost("create")]
        //public async Task<IActionResult> Create([FromBody] Category category)
        //{
        //    var created = await _categoryService.CreateAsync(category);
        //    return Ok(created);
        //}
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Category category)
        {
            try
            {
                var createdCategory = await _categoryService.CreateAsync(category);
                return CreatedAtAction(nameof(Get), new { id = createdCategory.Id }, createdCategory);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }


        // DELETE: api/category/delete/{id}
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _categoryService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
