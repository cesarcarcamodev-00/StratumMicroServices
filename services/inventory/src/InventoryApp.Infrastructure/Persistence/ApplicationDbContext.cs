using InventoryApp.Application.Interfaces;
using InventoryApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();
    public DbSet<UnitOfMeasure> UnitsOfMeasure => Set<UnitOfMeasure>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("inventory");

        // Product
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).HasMaxLength(200).IsRequired();
            entity.Property(p => p.SKU).HasMaxLength(50).IsRequired();
            entity.HasIndex(p => p.SKU).IsUnique();
            entity.Property(p => p.Description).HasMaxLength(2000);
            entity.Property(p => p.ReorderLevel).HasPrecision(18, 3).HasDefaultValue(10);
            entity.HasOne(p => p.Unit).WithMany(u => u.Products).HasForeignKey(p => p.UnitId).OnDelete(DeleteBehavior.Restrict);
        });

        // InventoryItem
        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Location).HasMaxLength(200).HasDefaultValue("Almacén Principal");
            entity.HasOne(i => i.Product).WithMany(p => p.InventoryItems).HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(i => new { i.ProductId, i.Location }).IsUnique();
        });

        // InventoryMovement
        modelBuilder.Entity<InventoryMovement>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.MovementType).HasConversion<string>().HasMaxLength(20);
            entity.Property(m => m.ReferenceType).HasMaxLength(50);
            entity.Property(m => m.Notes).HasMaxLength(500);
            entity.HasOne(m => m.Product).WithMany().HasForeignKey(m => m.ProductId).OnDelete(DeleteBehavior.Cascade);
        });

        // UnitOfMeasure
        modelBuilder.Entity<UnitOfMeasure>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Name).HasMaxLength(100).IsRequired();
            entity.Property(u => u.Symbol).HasMaxLength(20).IsRequired();
            entity.Property(u => u.Dimension).HasMaxLength(50).IsRequired();
            entity.Property(u => u.FactorToBase).HasPrecision(18, 6).HasDefaultValue(1);
            entity.Property(u => u.Description).HasMaxLength(500);
            entity.HasIndex(u => new { u.Name, u.Dimension }).IsUnique();
        });
    }
}
