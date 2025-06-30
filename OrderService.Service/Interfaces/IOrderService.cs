using OrderService.Service.Models;

namespace OrderService.Service.Interfaces
{
    public interface IOrderService
    {
        Task<ApiResponse<object>> CreateOrderAsync(int userId, OrderRequest orderRequest);
    }
}
