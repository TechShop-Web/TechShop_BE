using Grpc.Core;
using OrderService.Grpc;
using OrderService.Service.Interfaces;

namespace OrderService.API.Services
{
    public class OrderGrpcService : OrderService.Grpc.OrderService.OrderServiceBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderGrpcService> _logger;

        public OrderGrpcService(IOrderService orderService, ILogger<OrderGrpcService> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        public override async Task<GetOrderResponse> GetOrder(GetOrderRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("gRPC GetOrder called for OrderId: {OrderId}", request.OrderId);

                var orderResponse = await _orderService.GetOrderByIdAsync(request.OrderId);

                if (!orderResponse.Success || orderResponse.Data == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, $"Order {request.OrderId} not found"));
                }

                var order = orderResponse.Data;

                return new GetOrderResponse
                {
                    Id = order.Id,
                    OrderId = order.OrderId,
                    UserId = order.UserId,
                    OrderNumber = order.OrderNumber ?? "",
                    Status = order.Status ?? "",
                    Subtotal = (double)order.Subtotal,
                    ShippingCost = (double)order.ShippingCost,
                    TotalAmount = (double)order.TotalAmount,
                    ShippingAddress = order.ShippingAddress ?? "",
                    ShippingMethod = order.ShippingMethod ?? "",
                    Notes = order.Notes ?? "",
                    IsCancelled = order.IsCancelled,
                    CancelledReason = order.CancelledReason ?? "",
                    CancelledAt = order.CancelledAt?.ToString("O") ?? "",
                    ConfirmedAt = order.ConfirmedAt?.ToString("O") ?? "",
                    ShippedAt = order.ShippedAt?.ToString("O") ?? "",
                    DeliveredAt = order.DeliveredAt?.ToString("O") ?? "",
                    CreatedAt = order.CreatedAt.ToString("O"),
                    UpdatedAt = order.UpdatedAt.ToString("O")
                };
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in gRPC GetOrder for OrderId: {OrderId}", request.OrderId);
                throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
            }
        }
    }
}