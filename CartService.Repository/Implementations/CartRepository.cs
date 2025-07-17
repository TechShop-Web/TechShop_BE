using CartService.Repository.Interfaces;
using CartService.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Repository.Implementations
{
    public class CartRepository : GenericRepository<CartItem>, ICartRepository
    {
        public CartRepository(TechShopCartServiceDbContext context) : base(context)
        {
        }
    }
}
