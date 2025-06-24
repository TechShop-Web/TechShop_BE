using ProductService.Repository.Models;

namespace ProductService.Service.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<Category> CreateAsync(Category category);
        Task<bool> DeleteAsync(int id);
    }
}
