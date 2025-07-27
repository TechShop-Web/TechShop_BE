using CartService.Repository.Interfaces;
using CartService.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Repository.Implementations
{
    public class CartRepository : GenericRepository<CartItem>, ICartRepository
    {
        private readonly TechShopCartServiceDbContext _context;
        public CartRepository(TechShopCartServiceDbContext context) : base(context)
        {

        }

        //public Task<CartItem?> GetCartItemsByUserIdAsync(int userId)
        //{
        //    if (userId <= 0)
        //    {
        //        throw new ArgumentException("User ID must be a positive integer.", nameof(userId));
        //    }

        //    return _context.CartItems.FirstOrDefaultAsync(item => item.UserId == userId);
        //}
        public async Task<IEnumerable<CartItem>> GetAllCartAsync(string userId)
        {   if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            }
            return await _context.CartItems
                .ToListAsync();
        }
    }
}
