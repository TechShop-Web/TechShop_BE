using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Repository.Models;

namespace ProductService.Repository.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                // Laptops
                new Product
                {
                    Id = 1,
                    Name = "MacBook Air M2",
                    Description = "Apple MacBook Air với chip M2 mạnh mẽ, thiết kế siêu mỏng nhẹ",
                    Brand = "Apple",
                    CategoryId = 1,
                    CreatedAt = seedDate
                },
                new Product
                {
                    Id = 2,
                    Name = "Dell XPS 13",
                    Description = "Laptop Dell XPS 13 với màn hình InfinityEdge và hiệu năng vượt trội",
                    Brand = "Dell",
                    CategoryId = 1,
                    CreatedAt = seedDate
                },
                new Product
                {
                    Id = 3,
                    Name = "ASUS ROG Zephyrus G14",
                    Description = "Laptop gaming ASUS ROG với AMD Ryzen và RTX 4060",
                    Brand = "ASUS",
                    CategoryId = 1,
                    CreatedAt = seedDate
                },
                new Product
                {
                    Id = 4,
                    Name = "MSI Gaming GF63",
                    Description = "Laptop gaming MSI với Intel Core i5 và GTX 1650",
                    Brand = "MSI",
                    CategoryId = 1,
                    CreatedAt = seedDate
                },
                new Product
                {
                    Id = 5,
                    Name = "Lenovo ThinkPad X1 Carbon",
                    Description = "Laptop doanh nhân Lenovo ThinkPad siêu nhẹ và bền bỉ",
                    Brand = "Lenovo",
                    CategoryId = 1,
                    CreatedAt = seedDate
                },

                // Smartphones
                new Product
                {
                    Id = 6,
                    Name = "iPhone 15 Pro",
                    Description = "iPhone 15 Pro với chip A17 Pro và camera chuyên nghiệp",
                    Brand = "Apple",
                    CategoryId = 2,
                    CreatedAt = seedDate
                },
                new Product
                {
                    Id = 7,
                    Name = "Samsung Galaxy S24 Ultra",
                    Description = "Samsung Galaxy S24 Ultra với bút S Pen và camera 200MP",
                    Brand = "Samsung",
                    CategoryId = 2,
                    CreatedAt = seedDate
                },
                new Product
                {
                    Id = 8,
                    Name = "Xiaomi 13T Pro",
                    Description = "Xiaomi 13T Pro với Dimensity 9200+ và camera Leica",
                    Brand = "Xiaomi",
                    CategoryId = 2,
                    CreatedAt = seedDate
                },
                new Product
                {
                    Id = 9,
                    Name = "OPPO Find X6 Pro",
                    Description = "OPPO Find X6 Pro với camera Hasselblad và sạc nhanh 100W",
                    Brand = "OPPO",
                    CategoryId = 2,
                    CreatedAt = seedDate
                },
                new Product
                {
                    Id = 10,
                    Name = "Google Pixel 8 Pro",
                    Description = "Google Pixel 8 Pro với AI photography và chip Tensor G3",
                    Brand = "Google",
                    CategoryId = 2,
                    CreatedAt = seedDate
                },

                // Tablets
                new Product
                {
                    Id = 11,
                    Name = "iPad Pro 12.9",
                    Description = "iPad Pro 12.9 inch với chip M2 và màn hình Liquid Retina XDR",
                    Brand = "Apple",
                    CategoryId = 3,
                    CreatedAt = seedDate
                },
                new Product
                {
                    Id = 12,
                    Name = "Samsung Galaxy Tab S9",
                    Description = "Samsung Galaxy Tab S9 với màn hình Dynamic AMOLED 2X",
                    Brand = "Samsung",
                    CategoryId = 3,
                    CreatedAt = seedDate
                },
                new Product
                {
                    Id = 13,
                    Name = "Microsoft Surface Pro 9",
                    Description = "Microsoft Surface Pro 9 2-in-1 laptop tablet với Windows 11",
                    Brand = "Microsoft",
                    CategoryId = 3,
                    CreatedAt = seedDate
                },

                // Smartwatches
                new Product
                {
                    Id = 14,
                    Name = "Apple Watch Series 9",
                    Description = "Apple Watch Series 9 với chip S9 và màn hình Always-On Retina",
                    Brand = "Apple",
                    CategoryId = 4,
                    CreatedAt = seedDate
                },
                new Product
                {
                    Id = 15,
                    Name = "Samsung Galaxy Watch6",
                    Description = "Samsung Galaxy Watch6 với tính năng theo dõi sức khỏe toàn diện",
                    Brand = "Samsung",
                    CategoryId = 4,
                    CreatedAt = seedDate
                },

                // Headphones
                new Product
                {
                    Id = 16,
                    Name = "AirPods Pro 2",
                    Description = "AirPods Pro thế hệ 2 với chip H2 và chống ồn chủ động",
                    Brand = "Apple",
                    CategoryId = 5,
                    CreatedAt = seedDate
                },
                new Product
                {
                    Id = 17,
                    Name = "Sony WH-1000XM5",
                    Description = "Tai nghe Sony WH-1000XM5 với chống ồn hàng đầu",
                    Brand = "Sony",
                    CategoryId = 5,
                    CreatedAt = seedDate
                },
                new Product
                {
                    Id = 18,
                    Name = "Bose QuietComfort 45",
                    Description = "Tai nghe Bose QC45 với chống ồn tuyệt vời và âm thanh cân bằng",
                    Brand = "Bose",
                    CategoryId = 5,
                    CreatedAt = seedDate
                },

                // Gaming
                new Product
                {
                    Id = 19,
                    Name = "PlayStation 5",
                    Description = "Sony PlayStation 5 console với SSD tốc độ cao",
                    Brand = "Sony",
                    CategoryId = 6,
                    CreatedAt = seedDate
                },
                new Product
                {
                    Id = 20,
                    Name = "Nintendo Switch OLED",
                    Description = "Nintendo Switch phiên bản OLED với màn hình 7 inch",
                    Brand = "Nintendo",
                    CategoryId = 6,
                    CreatedAt = seedDate
                }
            );
        }
    }
}