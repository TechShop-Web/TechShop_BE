
using System.Text.Json.Serialization;

namespace ProductService.Repository.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;


        [JsonIgnore]
        public ICollection<Product>? Products { get; set; }
    }
}