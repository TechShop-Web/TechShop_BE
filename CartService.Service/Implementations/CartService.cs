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

        public async Task<ApiResponse<object>> AddItemToCartAsync(int userId, CartItem item)
        {
            await _cartRepository.AddAsync(item);
            await _unitOfWork.SaveChangesAsync();
            return ApiResponse<object>.Ok(new { ItemId = item.Id }, "Item added successfully.");
        }

 
        public async Task<ApiResponse<object>> CreateCartAsync(int userId, CartItem cartRequest)
        {

            var cartItems = new CartItem
            {
                UserId = userId,
                VariantId = cartRequest.VariantId,
                Quantity = cartRequest.Quantity,
                UnitPrice = cartRequest.UnitPrice,
                CreatedAt = DateTime.UtcNow
            };
            await _cartRepository.AddAsync(cartItems);
            await _unitOfWork.SaveChangesAsync();
            return ApiResponse<object>.Ok(new { CartId = cartItems.Id }, "Cart created successfully.");
        }

        public async Task<ApiResponse<object>> DeleteCartAsync(int userId, int CartId)
        {
            var cart = await _cartRepository.GetByIdAsync(CartId);
            if (cart == null || cart.UserId != userId) return ApiResponse<object>.Fail("Cart not found.");
            _cartRepository.Delete(cart);
            await _unitOfWork.SaveChangesAsync();
            return ApiResponse<object>.Ok(null, "Cart deleted successfully.");
        }

        public async Task<ApiResponse<CartItem>> GetCartByIdAsync(int CartId)
        {
            var cart = await _cartRepository.GetByIdAsync(CartId);
            return cart == null ? ApiResponse<CartItem>.Fail("Cart not found.") : ApiResponse<CartItem>.Ok(cart);
        }

        public async Task<ApiResponse<IEnumerable<CartItem>>> GetAllCartAsync(string userId)
        {
            var carts = await _cartRepository.GetAllCartAsync(userId);
            return carts == null || !carts.Any()
                ? ApiResponse<IEnumerable<CartItem>>.Fail("No carts found for this user.")
                : ApiResponse<IEnumerable<CartItem>>.Ok(carts.ToList());
        }


        public async Task<ApiResponse<object>> UpdateCartAsync(int userId, CartItem CartRequest)
        {
            var cart = await _cartRepository.GetByIdAsync(CartRequest.Id);
            if (cart == null || cart.UserId != userId) return ApiResponse<object>.Fail("Cart not found.");
            cart.VariantId = CartRequest.VariantId;
            cart.Quantity = CartRequest.Quantity;
            cart.UnitPrice = CartRequest.UnitPrice;
            await _unitOfWork.SaveChangesAsync();
            return ApiResponse<object>.Ok(null, "Cart updated successfully.");
        }
    }
}

