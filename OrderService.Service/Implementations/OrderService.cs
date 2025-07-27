using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderService.Repository.ApplicationContext;
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
        private readonly OrderContext _context;
        private readonly IMapper _mapper;
        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, OrderContext context)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        private async Task<int> GenerateNonDuplicateVnpOrderIdAsync(OrderContext context)
        {
            int candidate;
            bool exists;
            var rng = new Random();

            do
            {
                candidate = rng.Next(100_000_000, 2_000_000_000);
                exists = await context.Orders.AnyAsync(o => o.OrderId == candidate);
            }
            while (exists);

            return candidate;
        }

        public async Task<ApiResponse<object>> CancelOrderAsync(int orderId, string reason)
        {
            if (orderId <= 0)
            {
                throw new ArgumentException("Order ID must be greater than zero.", nameof(orderId));
            }
            if (string.IsNullOrWhiteSpace(reason))
            {
                throw new ArgumentException("Cancellation reason cannot be null or empty.", nameof(reason));
            }
            try
            {
                var order = await _unitOfWork.Orders
                    .Query()
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);
                if (order == null)
                {
                    return ApiResponse<object>.Fail("Order not found.");
                }
                if (order.IsCancelled)
                {
                    return ApiResponse<object>.Fail("Order is already cancelled.");
                }
                order.IsCancelled = true;
                order.CancelledReason = reason;
                order.CancelledAt = DateTime.UtcNow;
                order.Status = OrderStatus.Cancelled;
                _unitOfWork.Orders.Update(order);
                var result = await _unitOfWork.SaveChangesAsync();
                if (result <= 0)
                {
                    throw new Exception("Failed to cancel the order. Please try again.");
                }
                return ApiResponse<object>.Ok(new { orderId = order.OrderId }, "Order cancelled successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.Fail("An error occurred while cancelling the order.", ex.Message);
            }
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
                    OrderId = await GenerateNonDuplicateVnpOrderIdAsync(_context),
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

                return ApiResponse<object>.Ok(new { OrderId = newOrder.OrderId, OrderNumber = newOrder.OrderNumber }, "Order created successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.Fail("An error occurred while creating the order.", ex.Message);
            }
        }

        public async Task<ApiResponse<OrderMapperModel>> GetOrderByIdAsync(int orderId)
        {
            if (orderId <= 0)
            {
                throw new ArgumentException("Order ID must be greater than zero.", nameof(orderId));
            }

            try
            {
                var order = await _unitOfWork.Orders
                    .Query()
                    .Include(p => p.OrderItems)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                {
                    return ApiResponse<OrderMapperModel>.Fail("Order not found.");
                }

                var orderDto = _mapper.Map<OrderMapperModel>(order);

                return ApiResponse<OrderMapperModel>.Ok(orderDto, "Order retrieved successfully.");
            }
            catch (Exception)
            {
                return ApiResponse<OrderMapperModel>.Fail("An error occurred while retrieving the order.");
            }
        }

        public async Task<ApiResponse<List<OrderMapperModel>>> GetOrdersByUserIdAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("User ID must be greater than zero.", nameof(userId));
            }

            try
            {
                var orders = await _unitOfWork.Orders
                    .Query()
                    .Include(o => o.OrderItems)
                    .Where(o => o.UserId == userId)
                    .ToListAsync();

                if (!orders.Any())
                {
                    return ApiResponse<List<OrderMapperModel>>.Fail("No orders found for this user.");
                }

                var mappedOrders = _mapper.Map<List<OrderMapperModel>>(orders);

                return ApiResponse<List<OrderMapperModel>>.Ok(mappedOrders, "Orders retrieved successfully.");
            }
            catch (Exception)
            {
                return ApiResponse<List<OrderMapperModel>>.Fail("An error occurred while retrieving orders.");
            }
        }

        public async Task<ApiResponse<object>> UpdateOrderStatusAsync(int orderId, string status)
        {
            if (orderId <= 0)
            {
                throw new ArgumentException("Order ID must be greater than zero.", nameof(orderId));
            }
            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentException("Status cannot be null or empty.", nameof(status));
            }
            try
            {
                var order = await _unitOfWork.Orders
                    .Query()
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);
                if (order == null)
                {
                    return ApiResponse<object>.Fail("Order not found.");
                }
                if (!Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
                {
                    return ApiResponse<object>.Fail("Invalid order status.");
                }
                order.Status = orderStatus;
                order.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Orders.Update(order);
                var result = await _unitOfWork.SaveChangesAsync();
                if (result <= 0)
                {
                    throw new Exception("Failed to update the order status. Please try again.");
                }
                return ApiResponse<object>.Ok(new { orderId = order.OrderId, status = order.Status }, "Order status updated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.Fail("An error occurred while updating the order status.", ex.Message);
            }
        }

        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";
        }
    }
}
