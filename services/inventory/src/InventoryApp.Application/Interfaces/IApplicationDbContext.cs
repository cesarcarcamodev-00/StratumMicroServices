using InventoryApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    DbSet<InventoryItem> InventoryItems { get; }
    DbSet<InventoryMovement> InventoryMovements { get; }
    DbSet<UnitOfMeasure> UnitsOfMeasure { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
