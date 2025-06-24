using ProductService.Repository.Models;

namespace ProductService.Repository.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Product> ProductRepository { get; }
        IGenericRepository<Category> CategoryRepository { get; }
        IGenericRepository<ProductVariant> ProductVariantRepository { get; }

        Task<int> SaveChangesAsync();
    }
}
