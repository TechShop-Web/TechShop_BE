using System.ComponentModel.DataAnnotations;

namespace OrderService.Service.Models
{
    public class OrderRequest
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Subtotal must be a non-negative number.")]
        public decimal Subtotal { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Shipping cost must be a non-negative number.")]
        public decimal ShippingCost { get; set; }
        [Required]
        [StringLength(250, ErrorMessage = "Shipping address cannot exceed 250 characters.")]
        public string ShippingAddress { get; set; }
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        public string? Notes { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "At least one order item is required.")]
        public List<OrderItemRequest> OrderItems { get; set; } = new List<OrderItemRequest>();
    }

    public class OrderItemRequest
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int VariantId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }
    }
}
