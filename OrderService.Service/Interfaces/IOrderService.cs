using OrderService.Service.Models;

namespace OrderService.Service.Interfaces
{
    internal interface IOrderService
    {
        Task CreateOrderAsync(OrderRequest orderRequest);
    }
}
