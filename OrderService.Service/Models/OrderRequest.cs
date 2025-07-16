using System.ComponentModel.DataAnnotations;

namespace OrderService.Service.Models
{
    public class OrderRequest
    {
        [Required(ErrorMessage = "Subtotal is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Subtotal must be greater than 0")]
        public decimal Subtotal { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "ShippingCost cannot be negative")]
        public decimal ShippingCost { get; set; }

        [MaxLength(500, ErrorMessage = "ShippingAddress cannot exceed 500 characters")]
        public string? ShippingAddress { get; set; }

        [MaxLength(50, ErrorMessage = "ShippingMethod cannot exceed 50 characters")]
        public string? ShippingMethod { get; set; }

        public string? Notes { get; set; }

        [Required(ErrorMessage = "At least one order item is required")]
        [MinLength(1, ErrorMessage = "At least one order item is required")]
        public List<OrderItemRequest> OrderItems { get; set; } = new List<OrderItemRequest>();
    }

    public class OrderItemRequest
    {
        [Required(ErrorMessage = "ProductId is required")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "ProductName is required")]
        [MaxLength(255, ErrorMessage = "ProductName cannot exceed 255 characters")]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "VariantId is required")]
        public int VariantId { get; set; }

        [Required(ErrorMessage = "VariantName is required")]
        [MaxLength(255, ErrorMessage = "VariantName cannot exceed 255 characters")]
        public string VariantName { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "UnitPrice is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "UnitPrice must be greater than 0")]
        public decimal UnitPrice { get; set; }
    }
}
