using System.Text.Json.Serialization;

namespace ProductService.Repository.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Brand { get; set; }
        public DateTime CreatedAt { get; set; }

        public int CategoryId { get; set; }

        [JsonIgnore]
        public Category? Category { get; set; }
        [JsonIgnore]
        public ICollection<ProductVariant>? Variants { get; set; }
    }
}