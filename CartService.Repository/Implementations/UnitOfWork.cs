using CartService.Repository.Interfaces;
using CartService.Repository.Models;
using OrderService.Repository.Interfaces;

namespace CartService.Repository.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TechShopCartServiceDbContext _context;
        public IGenericRepository<CartItem> CartItems { get; }
        public UnitOfWork(TechShopCartServiceDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            CartItems = new GenericRepository<CartItem>(_context);
        }
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
