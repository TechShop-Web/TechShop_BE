using CartService.Repository.Interfaces;
using CartService.Repository.Models;
using CartService.Service.Interfaces;
using OrderService.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Service.Implementations
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CartService(ICartRepository cartRepository, IUnitOfWork unitOfWork)
        {
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
        }

        public Task<ApiResponse<object>> AddItemToCartAsync(int userId, CartItem item)
        {

        }

        public Task<ApiResponse<object>> ClearCartAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<object>> CreateCartAsync(int userId, CartItem CartRequest)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<object>> DeleteCartAsync(int userId, int CartId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<CartItem>> GetCartByIdAsync(int CartId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<CartItem>>> GetCartsByUserIdAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<object>> RemoveItemFromCartAsync(int userId, int itemId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<object>> UpdateCartAsync(int userId, CartItem CartRequest)
        {
            throw new NotImplementedException();
        }
    }
}
