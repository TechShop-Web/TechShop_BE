using System;
using System.Collections.Generic;

namespace CartService.Repository.Models;

public partial class CartItem
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? VariantId { get; set; }

    public int? Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public DateTime? CreatedAt { get; set; }
}
