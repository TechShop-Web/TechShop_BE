using OrderService.Repository.Interfaces;
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

        public Task CreateOrderAsync(OrderRequest orderRequest)
        {
            throw new NotImplementedException();
        }
    }
}
