
using CartService.Repository.Models;
using OrderService.Service.Models;

namespace CartService.Service.Interfaces
{
    public interface ICartService
    {
        Task<ApiResponse<object>> CreateCartAsync(int userId, CartItem CartRequest);
        Task<ApiResponse<CartItem>> GetCartByIdAsync(int CartId);
        Task<ApiResponse<CartItem>> GetCartsByUserIdAsync(int userId);
        Task<ApiResponse<object>> UpdateCartAsync(int userId, CartItem CartRequest);
        Task<ApiResponse<object>> DeleteCartAsync(int userId, int CartId);
        Task<ApiResponse<object>> ClearCartAsync(int userId);
        Task<ApiResponse<object>> AddItemToCartAsync(int userId, CartItem item);


    }
}
