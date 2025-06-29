using OrderService.Service.Models;

namespace OrderService.Service.Interfaces
{
    public interface IOrderService
    {
        Task CreateOrderAsync(OrderRequest orderRequest);
    }
}
