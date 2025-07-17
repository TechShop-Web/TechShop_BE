using CartService.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Service.Models
{
    public class CartRequest
    {
        public int? UserId { get; set; }

        public int? VariantId { get; set; }

        public int? Quantity { get; set; }

        public decimal? UnitPrice { get; set; }
    }
}
