using ProductService.Repository.Frameworks;
using ProductService.Repository.Models;

namespace ProductService.Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        private IGenericRepository<Product>? _productRepo;
        private IGenericRepository<Category>? _categoryRepo;
        private IGenericRepository<ProductVariant>? _variantRepo;

        public IGenericRepository<Product> ProductRepository =>
            _productRepo ??= new GenericRepository<Product>(_context);

        public IGenericRepository<Category> CategoryRepository =>
            _categoryRepo ??= new GenericRepository<Category>(_context);

        public IGenericRepository<ProductVariant> ProductVariantRepository =>
            _variantRepo ??= new GenericRepository<ProductVariant>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
