using OrderService.Repository.ApplicationContext;
using OrderService.Repository.Interfaces;
using OrderService.Repository.Models;

namespace OrderService.Repository.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OrderContext _context;
        public IGenericRepository<Order> Orders { get; }
        public IGenericRepository<OrderItem> OrderItems { get; }
        public UnitOfWork(OrderContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Orders = new GenericRepository<Order>(_context);
            OrderItems = new GenericRepository<OrderItem>(_context);
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
