using CartService.Repository.Models;
using OrderService.Repository.Interfaces;

namespace CartService.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<CartItem> CartItems { get; }
        Task<int> SaveChangesAsync();
    }
}
