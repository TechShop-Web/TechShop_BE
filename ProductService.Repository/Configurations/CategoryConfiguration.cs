using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Repository.Models;

namespace ProductService.Repository.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasData(
                new Category { Id = 1, Name = "Laptop" },
                new Category { Id = 2, Name = "Smartphone" },
                new Category { Id = 3, Name = "Tablet" },
                new Category { Id = 4, Name = "Smartwatch" },
                new Category { Id = 5, Name = "Headphones" },
                new Category { Id = 6, Name = "Gaming" },
                new Category { Id = 7, Name = "Accessories" }
            );
        }
    }
}