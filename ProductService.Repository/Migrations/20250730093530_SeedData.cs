using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductService.Repository.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Laptop" },
                    { 2, "Smartphone" },
                    { 3, "Tablet" },
                    { 4, "Smartwatch" },
                    { 5, "Headphones" },
                    { 6, "Gaming" },
                    { 7, "Accessories" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Brand", "CategoryId", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Apple", 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Apple MacBook Air với chip M2 mạnh mẽ, thiết kế siêu mỏng nhẹ", "MacBook Air M2" },
                    { 2, "Dell", 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Laptop Dell XPS 13 với màn hình InfinityEdge và hiệu năng vượt trội", "Dell XPS 13" },
                    { 3, "ASUS", 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Laptop gaming ASUS ROG với AMD Ryzen và RTX 4060", "ASUS ROG Zephyrus G14" },
                    { 4, "MSI", 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Laptop gaming MSI với Intel Core i5 và GTX 1650", "MSI Gaming GF63" },
                    { 5, "Lenovo", 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Laptop doanh nhân Lenovo ThinkPad siêu nhẹ và bền bỉ", "Lenovo ThinkPad X1 Carbon" },
                    { 6, "Apple", 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "iPhone 15 Pro với chip A17 Pro và camera chuyên nghiệp", "iPhone 15 Pro" },
                    { 7, "Samsung", 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Samsung Galaxy S24 Ultra với bút S Pen và camera 200MP", "Samsung Galaxy S24 Ultra" },
                    { 8, "Xiaomi", 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Xiaomi 13T Pro với Dimensity 9200+ và camera Leica", "Xiaomi 13T Pro" },
                    { 9, "OPPO", 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "OPPO Find X6 Pro với camera Hasselblad và sạc nhanh 100W", "OPPO Find X6 Pro" },
                    { 10, "Google", 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Google Pixel 8 Pro với AI photography và chip Tensor G3", "Google Pixel 8 Pro" },
                    { 11, "Apple", 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "iPad Pro 12.9 inch với chip M2 và màn hình Liquid Retina XDR", "iPad Pro 12.9" },
                    { 12, "Samsung", 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Samsung Galaxy Tab S9 với màn hình Dynamic AMOLED 2X", "Samsung Galaxy Tab S9" },
                    { 13, "Microsoft", 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Microsoft Surface Pro 9 2-in-1 laptop tablet với Windows 11", "Microsoft Surface Pro 9" },
                    { 14, "Apple", 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Apple Watch Series 9 với chip S9 và màn hình Always-On Retina", "Apple Watch Series 9" },
                    { 15, "Samsung", 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Samsung Galaxy Watch6 với tính năng theo dõi sức khỏe toàn diện", "Samsung Galaxy Watch6" },
                    { 16, "Apple", 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AirPods Pro thế hệ 2 với chip H2 và chống ồn chủ động", "AirPods Pro 2" },
                    { 17, "Sony", 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tai nghe Sony WH-1000XM5 với chống ồn hàng đầu", "Sony WH-1000XM5" },
                    { 18, "Bose", 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tai nghe Bose QC45 với chống ồn tuyệt vời và âm thanh cân bằng", "Bose QuietComfort 45" },
                    { 19, "Sony", 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sony PlayStation 5 console với SSD tốc độ cao", "PlayStation 5" },
                    { 20, "Nintendo", 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Nintendo Switch phiên bản OLED với màn hình 7 inch", "Nintendo Switch OLED" }
                });

            migrationBuilder.InsertData(
                table: "ProductVariants",
                columns: new[] { "Id", "ConfigLabel", "CreatedAt", "Price", "ProductId", "Stock" },
                values: new object[,]
                {
                    { 1, "8GB RAM, 256GB SSD - Midnight", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 32990000m, 1, 15 },
                    { 2, "8GB RAM, 512GB SSD - Silver", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 39990000m, 1, 10 },
                    { 3, "16GB RAM, 512GB SSD - Space Gray", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 46990000m, 1, 8 },
                    { 4, "Intel i5, 8GB RAM, 256GB SSD", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 25990000m, 2, 12 },
                    { 5, "Intel i7, 16GB RAM, 512GB SSD", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 34990000m, 2, 7 },
                    { 6, "Ryzen 7, 16GB RAM, RTX 4060", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 42990000m, 3, 6 },
                    { 7, "Ryzen 9, 32GB RAM, RTX 4070", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 52990000m, 3, 4 },
                    { 8, "Intel i5, 8GB RAM, GTX 1650", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 18990000m, 4, 10 },
                    { 9, "Intel i7, 16GB RAM, RTX 3050", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 23990000m, 4, 8 },
                    { 10, "Intel i7, 16GB RAM, 512GB SSD", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 45990000m, 5, 7 },
                    { 11, "Intel i7, 32GB RAM, 1TB SSD", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 55990000m, 5, 5 },
                    { 12, "128GB - Natural Titanium", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 28990000m, 6, 25 },
                    { 13, "256GB - Blue Titanium", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 33990000m, 6, 20 },
                    { 14, "512GB - White Titanium", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 41990000m, 6, 15 },
                    { 15, "1TB - Black Titanium", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 49990000m, 6, 10 },
                    { 16, "256GB - Titanium Black", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 31990000m, 7, 18 },
                    { 17, "512GB - Titanium Violet", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 37990000m, 7, 12 },
                    { 18, "1TB - Titanium Gray", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 45990000m, 7, 6 },
                    { 19, "256GB - Black", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 12990000m, 8, 22 },
                    { 20, "512GB - Blue", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 15990000m, 8, 16 },
                    { 21, "512GB - Green", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 15990000m, 8, 14 },
                    { 22, "256GB - Gold", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 22990000m, 9, 14 },
                    { 23, "512GB - Brown", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 26990000m, 9, 10 },
                    { 24, "128GB - Obsidian", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 24990000m, 10, 16 },
                    { 25, "256GB - Porcelain", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 28990000m, 10, 12 },
                    { 26, "512GB - Bay", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 33990000m, 10, 8 },
                    { 27, "128GB Wi-Fi - Space Gray", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 27990000m, 11, 14 },
                    { 28, "256GB Wi-Fi + Cellular - Silver", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 35990000m, 11, 9 },
                    { 29, "512GB Wi-Fi - Space Gray", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 41990000m, 11, 6 },
                    { 30, "128GB Wi-Fi - Graphite", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 19990000m, 12, 16 },
                    { 31, "256GB 5G - Beige", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 24990000m, 12, 11 },
                    { 32, "512GB 5G - Cream", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 29990000m, 12, 8 },
                    { 33, "Intel i5, 8GB RAM, 256GB SSD", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 26990000m, 13, 12 },
                    { 34, "Intel i7, 16GB RAM, 512GB SSD", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 35990000m, 13, 8 },
                    { 35, "41mm GPS - Pink", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9990000m, 14, 22 },
                    { 36, "45mm GPS + Cellular - Midnight", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 12990000m, 14, 18 },
                    { 37, "41mm GPS - Starlight", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9990000m, 14, 20 },
                    { 38, "40mm Bluetooth - Graphite", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6990000m, 15, 25 },
                    { 39, "44mm LTE - Silver", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8990000m, 15, 20 },
                    { 40, "40mm Bluetooth - Gold", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6990000m, 15, 22 },
                    { 41, "USB-C - White", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6490000m, 16, 30 },
                    { 42, "Wireless - Black", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8990000m, 17, 15 },
                    { 43, "Wireless - Silver", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8990000m, 17, 12 },
                    { 44, "Wireless - Black", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7990000m, 18, 18 },
                    { 45, "Wireless - White Smoke", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7990000m, 18, 15 },
                    { 46, "Standard Edition", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 13990000m, 19, 8 },
                    { 47, "Digital Edition", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10990000m, 19, 5 },
                    { 48, "White - OLED Model", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8990000m, 20, 20 },
                    { 49, "Neon Blue/Red - OLED Model", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8990000m, 20, 18 },
                    { 50, "Splatoon 3 Edition - OLED Model", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9990000m, 20, 12 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
