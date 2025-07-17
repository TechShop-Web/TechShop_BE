using CartService.Repository.Models;
using OrderService.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Repository.Interfaces
{
    public interface ICartRepository: IGenericRepository<CartItem>
    {
    }
}
