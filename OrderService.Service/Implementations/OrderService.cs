using OrderService.Repository.Enum;
using OrderService.Repository.Interfaces;
using OrderService.Repository.Models;
using OrderService.Service.Interfaces;
using OrderService.Service.Models;

namespace OrderService.Service.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ApiResponse<object>> CreateOrderAsync(int userId, OrderRequest orderRequest)
        {
            if (orderRequest == null)
            {
                throw new ArgumentNullException(nameof(orderRequest), "Order request cannot be null");
            }

            if (orderRequest.OrderItems == null || orderRequest.OrderItems.Count == 0)
            {
                throw new ArgumentException("Order must contain at least one item", nameof(orderRequest.OrderItems));
            }


            try
            {
                var newOrder = new Order
                {
                    UserId = userId,
                    OrderNumber = GenerateOrderNumber(),
                    Status = OrderStatus.Pending,
                    Subtotal = orderRequest.Subtotal,
                    ShippingCost = orderRequest.ShippingCost,
                    TotalAmount = orderRequest.Subtotal + orderRequest.ShippingCost,
                    ShippingAddress = orderRequest.ShippingAddress,
                    ShippingMethod = orderRequest.ShippingMethod,
                    Notes = orderRequest.Notes,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsCancelled = false,
                    OrderItems = orderRequest.OrderItems.Select(item => new OrderItem
                    {
                        ProductId = item.ProductId,
                        VariantId = item.VariantId,
                        ProductName = item.ProductName,
                        VariantName = item.VariantName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.UnitPrice * item.Quantity,
                        CreatedAt = DateTime.UtcNow,
                    }).ToList()
                };

                await _unitOfWork.Orders.AddAsync(newOrder);
                var result = await _unitOfWork.SaveChangesAsync();

                if (result <= 0)
                {
                    throw new Exception("Failed to create order. Please try again.");
                }

                return ApiResponse<object>.Ok(new { OrderId = newOrder.Id, OrderNumber = newOrder.OrderNumber }, "Order created successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.Fail("An error occurred while creating the order.", ex.Message);
            }
        }
        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";
        }
    }
}
