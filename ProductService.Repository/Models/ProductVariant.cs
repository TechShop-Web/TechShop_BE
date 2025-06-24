namespace ProductService.Repository.Models
{
    public class ProductVariant
    {
        public int Id { get; set; }
        public string ConfigLabel { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public DateTime CreatedAt { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
