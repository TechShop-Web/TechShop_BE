using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CartService.Repository.Models;

public partial class TechShopCartServiceDbContext : DbContext
{
    public TechShopCartServiceDbContext()
    {
    }

    public TechShopCartServiceDbContext(DbContextOptions<TechShopCartServiceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CartItem> CartItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CartItem__3214EC07100864B5");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
