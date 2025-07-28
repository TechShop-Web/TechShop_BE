using OrderService.Service.Models;

namespace OrderService.Service.Interfaces
{
    public interface IOrderStatisticsService
    {
        Task<ApiResponse<PaginatedList<OrderStatisticsModel>>> GetOrderStatisticsAsync(DateTime? startDate, DateTime? endDate, int pageIndex, int pageSize);
    }
}
