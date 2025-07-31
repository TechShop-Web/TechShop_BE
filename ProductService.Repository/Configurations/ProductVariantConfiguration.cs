using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Repository.Models;

namespace ProductService.Repository.Configurations
{
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            
            var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                // MacBook Air M2 variants (ProductId = 1)
                new ProductVariant { Id = 1, ProductId = 1, ConfigLabel = "8GB RAM, 256GB SSD - Midnight", Price = 32990000m, Stock = 15, CreatedAt = seedDate },
                new ProductVariant { Id = 2, ProductId = 1, ConfigLabel = "8GB RAM, 512GB SSD - Silver", Price = 39990000m, Stock = 10, CreatedAt = seedDate },
                new ProductVariant { Id = 3, ProductId = 1, ConfigLabel = "16GB RAM, 512GB SSD - Space Gray", Price = 46990000m, Stock = 8, CreatedAt = seedDate },

                // Dell XPS 13 variants (ProductId = 2)
                new ProductVariant { Id = 4, ProductId = 2, ConfigLabel = "Intel i5, 8GB RAM, 256GB SSD", Price = 25990000m, Stock = 12, CreatedAt = seedDate },
                new ProductVariant { Id = 5, ProductId = 2, ConfigLabel = "Intel i7, 16GB RAM, 512GB SSD", Price = 34990000m, Stock = 7, CreatedAt = seedDate },

                // ASUS ROG Zephyrus G14 variants (ProductId = 3)
                new ProductVariant { Id = 6, ProductId = 3, ConfigLabel = "Ryzen 7, 16GB RAM, RTX 4060", Price = 42990000m, Stock = 6, CreatedAt = seedDate },
                new ProductVariant { Id = 7, ProductId = 3, ConfigLabel = "Ryzen 9, 32GB RAM, RTX 4070", Price = 52990000m, Stock = 4, CreatedAt = seedDate },

                // MSI Gaming GF63 variants (ProductId = 4)
                new ProductVariant { Id = 8, ProductId = 4, ConfigLabel = "Intel i5, 8GB RAM, GTX 1650", Price = 18990000m, Stock = 10, CreatedAt = seedDate },
                new ProductVariant { Id = 9, ProductId = 4, ConfigLabel = "Intel i7, 16GB RAM, RTX 3050", Price = 23990000m, Stock = 8, CreatedAt = seedDate },

                // Lenovo ThinkPad X1 Carbon variants (ProductId = 5)
                new ProductVariant { Id = 10, ProductId = 5, ConfigLabel = "Intel i7, 16GB RAM, 512GB SSD", Price = 45990000m, Stock = 7, CreatedAt = seedDate },
                new ProductVariant { Id = 11, ProductId = 5, ConfigLabel = "Intel i7, 32GB RAM, 1TB SSD", Price = 55990000m, Stock = 5, CreatedAt = seedDate },

                // iPhone 15 Pro variants (ProductId = 6)
                new ProductVariant { Id = 12, ProductId = 6, ConfigLabel = "128GB - Natural Titanium", Price = 28990000m, Stock = 25, CreatedAt = seedDate },
                new ProductVariant { Id = 13, ProductId = 6, ConfigLabel = "256GB - Blue Titanium", Price = 33990000m, Stock = 20, CreatedAt = seedDate },
                new ProductVariant { Id = 14, ProductId = 6, ConfigLabel = "512GB - White Titanium", Price = 41990000m, Stock = 15, CreatedAt = seedDate },
                new ProductVariant { Id = 15, ProductId = 6, ConfigLabel = "1TB - Black Titanium", Price = 49990000m, Stock = 10, CreatedAt = seedDate },

                // Samsung Galaxy S24 Ultra variants (ProductId = 7)
                new ProductVariant { Id = 16, ProductId = 7, ConfigLabel = "256GB - Titanium Black", Price = 31990000m, Stock = 18, CreatedAt = seedDate },
                new ProductVariant { Id = 17, ProductId = 7, ConfigLabel = "512GB - Titanium Violet", Price = 37990000m, Stock = 12, CreatedAt = seedDate },
                new ProductVariant { Id = 18, ProductId = 7, ConfigLabel = "1TB - Titanium Gray", Price = 45990000m, Stock = 6, CreatedAt = seedDate },

                // Xiaomi 13T Pro variants (ProductId = 8)
                new ProductVariant { Id = 19, ProductId = 8, ConfigLabel = "256GB - Black", Price = 12990000m, Stock = 22, CreatedAt = seedDate },
                new ProductVariant { Id = 20, ProductId = 8, ConfigLabel = "512GB - Blue", Price = 15990000m, Stock = 16, CreatedAt = seedDate },
                new ProductVariant { Id = 21, ProductId = 8, ConfigLabel = "512GB - Green", Price = 15990000m, Stock = 14, CreatedAt = seedDate },

                // OPPO Find X6 Pro variants (ProductId = 9)
                new ProductVariant { Id = 22, ProductId = 9, ConfigLabel = "256GB - Gold", Price = 22990000m, Stock = 14, CreatedAt = seedDate },
                new ProductVariant { Id = 23, ProductId = 9, ConfigLabel = "512GB - Brown", Price = 26990000m, Stock = 10, CreatedAt = seedDate },

                // Google Pixel 8 Pro variants (ProductId = 10)
                new ProductVariant { Id = 24, ProductId = 10, ConfigLabel = "128GB - Obsidian", Price = 24990000m, Stock = 16, CreatedAt = seedDate },
                new ProductVariant { Id = 25, ProductId = 10, ConfigLabel = "256GB - Porcelain", Price = 28990000m, Stock = 12, CreatedAt = seedDate },
                new ProductVariant { Id = 26, ProductId = 10, ConfigLabel = "512GB - Bay", Price = 33990000m, Stock = 8, CreatedAt = seedDate },

                // iPad Pro 12.9 variants (ProductId = 11)
                new ProductVariant { Id = 27, ProductId = 11, ConfigLabel = "128GB Wi-Fi - Space Gray", Price = 27990000m, Stock = 14, CreatedAt = seedDate },
                new ProductVariant { Id = 28, ProductId = 11, ConfigLabel = "256GB Wi-Fi + Cellular - Silver", Price = 35990000m, Stock = 9, CreatedAt = seedDate },
                new ProductVariant { Id = 29, ProductId = 11, ConfigLabel = "512GB Wi-Fi - Space Gray", Price = 41990000m, Stock = 6, CreatedAt = seedDate },

                // Samsung Galaxy Tab S9 variants (ProductId = 12)
                new ProductVariant { Id = 30, ProductId = 12, ConfigLabel = "128GB Wi-Fi - Graphite", Price = 19990000m, Stock = 16, CreatedAt = seedDate },
                new ProductVariant { Id = 31, ProductId = 12, ConfigLabel = "256GB 5G - Beige", Price = 24990000m, Stock = 11, CreatedAt = seedDate },
                new ProductVariant { Id = 32, ProductId = 12, ConfigLabel = "512GB 5G - Cream", Price = 29990000m, Stock = 8, CreatedAt = seedDate },

                // Microsoft Surface Pro 9 variants (ProductId = 13)
                new ProductVariant { Id = 33, ProductId = 13, ConfigLabel = "Intel i5, 8GB RAM, 256GB SSD", Price = 26990000m, Stock = 12, CreatedAt = seedDate },
                new ProductVariant { Id = 34, ProductId = 13, ConfigLabel = "Intel i7, 16GB RAM, 512GB SSD", Price = 35990000m, Stock = 8, CreatedAt = seedDate },

                // Apple Watch Series 9 variants (ProductId = 14)
                new ProductVariant { Id = 35, ProductId = 14, ConfigLabel = "41mm GPS - Pink", Price = 9990000m, Stock = 22, CreatedAt = seedDate },
                new ProductVariant { Id = 36, ProductId = 14, ConfigLabel = "45mm GPS + Cellular - Midnight", Price = 12990000m, Stock = 18, CreatedAt = seedDate },
                new ProductVariant { Id = 37, ProductId = 14, ConfigLabel = "41mm GPS - Starlight", Price = 9990000m, Stock = 20, CreatedAt = seedDate },

                // Samsung Galaxy Watch6 variants (ProductId = 15)
                new ProductVariant { Id = 38, ProductId = 15, ConfigLabel = "40mm Bluetooth - Graphite", Price = 6990000m, Stock = 25, CreatedAt = seedDate },
                new ProductVariant { Id = 39, ProductId = 15, ConfigLabel = "44mm LTE - Silver", Price = 8990000m, Stock = 20, CreatedAt = seedDate },
                new ProductVariant { Id = 40, ProductId = 15, ConfigLabel = "40mm Bluetooth - Gold", Price = 6990000m, Stock = 22, CreatedAt = seedDate },

                // AirPods Pro 2 variants (ProductId = 16)
                new ProductVariant { Id = 41, ProductId = 16, ConfigLabel = "USB-C - White", Price = 6490000m, Stock = 30, CreatedAt = seedDate },

                // Sony WH-1000XM5 variants (ProductId = 17)
                new ProductVariant { Id = 42, ProductId = 17, ConfigLabel = "Wireless - Black", Price = 8990000m, Stock = 15, CreatedAt = seedDate },
                new ProductVariant { Id = 43, ProductId = 17, ConfigLabel = "Wireless - Silver", Price = 8990000m, Stock = 12, CreatedAt = seedDate },

                // Bose QuietComfort 45 variants (ProductId = 18)
                new ProductVariant { Id = 44, ProductId = 18, ConfigLabel = "Wireless - Black", Price = 7990000m, Stock = 18, CreatedAt = seedDate },
                new ProductVariant { Id = 45, ProductId = 18, ConfigLabel = "Wireless - White Smoke", Price = 7990000m, Stock = 15, CreatedAt = seedDate },

                // PlayStation 5 variants (ProductId = 19)
                new ProductVariant { Id = 46, ProductId = 19, ConfigLabel = "Standard Edition", Price = 13990000m, Stock = 8, CreatedAt = seedDate },
                new ProductVariant { Id = 47, ProductId = 19, ConfigLabel = "Digital Edition", Price = 10990000m, Stock = 5, CreatedAt = seedDate },

                // Nintendo Switch OLED variants (ProductId = 20)
                new ProductVariant { Id = 48, ProductId = 20, ConfigLabel = "White - OLED Model", Price = 8990000m, Stock = 20, CreatedAt = seedDate },
                new ProductVariant { Id = 49, ProductId = 20, ConfigLabel = "Neon Blue/Red - OLED Model", Price = 8990000m, Stock = 18, CreatedAt = seedDate },
                new ProductVariant { Id = 50, ProductId = 20, ConfigLabel = "Splatoon 3 Edition - OLED Model", Price = 9990000m, Stock = 12, CreatedAt = seedDate }
            );
        }
    }
}