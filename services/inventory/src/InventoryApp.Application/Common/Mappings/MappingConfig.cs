using InventoryApp.Application.DTOs;
using InventoryApp.Domain.Entities;
using Mapster;

namespace InventoryApp.Application.Common.Mappings;

public static class MappingConfig
{
    public static void Configure(TypeAdapterConfig config)
    {
        config.NewConfig<Product, ProductDto>()
            .Map(d => d.UnitName, s => s.Unit != null ? s.Unit.Name : null)
            .Map(d => d.UnitSymbol, s => s.Unit != null ? s.Unit.Symbol : null)
            .Map(d => d.CurrentStock, s => s.InventoryItems.Sum(i => i.QuantityOnHand));

        config.NewConfig<UnitOfMeasure, UnitOfMeasureDto>()
            .Map(d => d.ProductCount, s => s.Products.Count(p => p.IsActive));

        config.NewConfig<InventoryMovement, InventoryMovementDto>()
            .Map(d => d.ProductName, s => s.Product != null ? s.Product.Name : "");

        config.NewConfig<InventoryItem, InventoryItemDto>()
            .Map(d => d.ProductName, s => s.Product != null ? s.Product.Name : null)
            .Map(d => d.ProductSKU, s => s.Product != null ? s.Product.SKU : null);
    }
}
