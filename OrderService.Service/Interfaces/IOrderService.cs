using OrderService.Service.Models;

namespace OrderService.Service.Interfaces
{
    public interface IOrderService
    {
        Task<ApiResponse<object>> CreateOrderAsync(int userId, OrderRequest orderRequest);
        Task<ApiResponse<OrderMapperModel>> GetOrderByIdAsync(int orderId);
        Task<ApiResponse<List<OrderMapperModel>>> GetOrdersByUserIdAsync(int userId);
    }
}
