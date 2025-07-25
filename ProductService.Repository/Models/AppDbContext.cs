using Microsoft.EntityFrameworkCore;
using ProductService.Repository.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ProductService.Repository.Frameworks
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình Category
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();

            // Cấu hình Product
            modelBuilder.Entity<Product>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            // Cấu hình ProductVariant
            modelBuilder.Entity<ProductVariant>()
                .Property(v => v.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<ProductVariant>()
                .Property(v => v.Price)
                .HasPrecision(10, 2);
        }

    }
}