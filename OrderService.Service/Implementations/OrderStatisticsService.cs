using Microsoft.EntityFrameworkCore;
using OrderService.Repository.ApplicationContext;
using OrderService.Service.Interfaces;
using OrderService.Service.Models;

namespace OrderService.Service.Implementations
{
    public class OrderStatisticsService : IOrderStatisticsService
    {
        private readonly OrderContext _context;

        public OrderStatisticsService(OrderContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ApiResponse<PaginatedList<OrderStatisticsModel>>> GetOrderStatisticsAsync(DateTime? startDate, DateTime? endDate, int pageIndex, int pageSize)
        {
            if (pageIndex <= 0 || pageSize <= 0)
            {
                throw new ArgumentException("Page index and page size must be greater than zero.");
            }

            var query = _context.Orders.AsQueryable();

            // Filter by date range if provided
            if (startDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt <= endDate.Value);
            }

            // Group by date (day, month, year) and calculate statistics
            var groupedData = query
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month, o.CreatedAt.Day })
                .Select(g => new OrderStatisticsModel
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Day = g.Key.Day,
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Sum(o => o.TotalAmount)
                });

            // Pagination
            var totalRecords = await groupedData.CountAsync();
            var paginatedData = await groupedData
                .OrderByDescending(o => o.Year)
                .ThenByDescending(o => o.Month)
                .ThenByDescending(o => o.Day)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var paginatedList = new PaginatedList<OrderStatisticsModel>(paginatedData, totalRecords, pageIndex, pageSize);

            return ApiResponse<PaginatedList<OrderStatisticsModel>>.Ok(paginatedList, "Statistics retrieved successfully.");
        }
    }
}
