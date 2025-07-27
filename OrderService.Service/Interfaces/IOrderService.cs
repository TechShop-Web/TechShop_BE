using OrderService.Service.Models;

namespace OrderService.Service.Interfaces
{
    public interface IOrderService
    {
        Task<ApiResponse<object>> CreateOrderAsync(int userId, OrderRequest orderRequest);
        Task<ApiResponse<OrderMapperModel>> GetOrderByIdAsync(int orderId);
        Task<ApiResponse<List<OrderMapperModel>>> GetOrdersByUserIdAsync(int userId);
        Task<ApiResponse<object>> UpdateOrderStatusAsync(int orderId, string status);
        Task<ApiResponse<object>> CancelOrderAsync(int orderId, string reason);
    }
}
