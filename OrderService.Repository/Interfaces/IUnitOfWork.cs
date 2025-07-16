using OrderService.Repository.Models;

namespace OrderService.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Order> Orders { get; }
        IGenericRepository<OrderItem> OrderItems { get; }
        Task<int> SaveChangesAsync();
    }
}
