using ProductService.Repository.Models;

namespace ProductService.Service.Services
{
    public interface IProductVariantService
    {
        Task<IEnumerable<ProductVariant>> GetAllAsync();
        Task<ProductVariant?> GetByIdAsync(int id);
        Task<ProductVariant> CreateAsync(ProductVariant variant);
        Task<bool> UpdateAsync(ProductVariant variant);
        Task<bool> DeleteAsync(int id);
    }
}
