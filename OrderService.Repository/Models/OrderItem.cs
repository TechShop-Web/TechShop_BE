using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Repository.Models
{
    public class OrderItem
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int VariantId { get; set; }
        [Required]
        [MaxLength(255)]
        public string ProductName { get; set; }
        [Required]
        [MaxLength(255)]
        public string VariantName { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
    }
}
