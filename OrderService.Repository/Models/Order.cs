using OrderService.Repository.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Repository.Models
{
    public class Order
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [MaxLength(50)]
        public string OrderNumber { get; set; }
        [Required]
        public OrderStatus Status { get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal ShippingCost { get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; }
        public string? Notes { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
