using InventoryApp.Domain.Entities;

namespace InventoryApp.Infrastructure.Persistence;

public static class SeedConstants
{
    // Weight (base: kilogramo)
    public static readonly Guid KgId = Guid.Parse("10000000-0000-0000-0000-000000000001");
    public static readonly Guid LbId = Guid.Parse("10000000-0000-0000-0000-000000000002");
    public static readonly Guid GramId = Guid.Parse("10000000-0000-0000-0000-000000000003");
    public static readonly Guid OzId = Guid.Parse("10000000-0000-0000-0000-000000000004");

    // Volume (base: litro)
    public static readonly Guid LiterId = Guid.Parse("20000000-0000-0000-0000-000000000001");
    public static readonly Guid GallonId = Guid.Parse("20000000-0000-0000-0000-000000000002");

    // Count (base: unidad)
    public static readonly Guid UnitId = Guid.Parse("30000000-0000-0000-0000-000000000001");
    public static readonly Guid PieceId = Guid.Parse("30000000-0000-0000-0000-000000000002");
    public static readonly Guid PackageId = Guid.Parse("30000000-0000-0000-0000-000000000003");
    public static readonly Guid BoxId = Guid.Parse("30000000-0000-0000-0000-000000000004");
    public static readonly Guid DozenId = Guid.Parse("30000000-0000-0000-0000-000000000005");

    public static List<UnitOfMeasure> DefaultUnits() => new()
    {
        new UnitOfMeasure { Id = KgId, Name = "Kilogramo", Symbol = "kg", Dimension = "Weight", FactorToBase = 1m, IsBaseUnit = true, Description = "Kilogramo (unidad base de peso)" },
        new UnitOfMeasure { Id = LbId, Name = "Libra", Symbol = "lb", Dimension = "Weight", FactorToBase = 0.45359237m, Description = "Libra avoirdupois" },
        new UnitOfMeasure { Id = GramId, Name = "Gramo", Symbol = "g", Dimension = "Weight", FactorToBase = 0.001m, Description = "Gramo" },
        new UnitOfMeasure { Id = OzId, Name = "Onza", Symbol = "oz", Dimension = "Weight", FactorToBase = 0.028349523125m, Description = "Onza avoirdupois" },
        new UnitOfMeasure { Id = LiterId, Name = "Litro", Symbol = "L", Dimension = "Volume", FactorToBase = 1m, IsBaseUnit = true, Description = "Litro (unidad base de volumen)" },
        new UnitOfMeasure { Id = GallonId, Name = "Galón", Symbol = "gal", Dimension = "Volume", FactorToBase = 3.78541178m, Description = "Galón estadounidense" },
        new UnitOfMeasure { Id = UnitId, Name = "Unidad", Symbol = "ud", Dimension = "Count", FactorToBase = 1m, IsBaseUnit = true, Description = "Unidad individual (unidad base de conteo)" },
        new UnitOfMeasure { Id = PieceId, Name = "Pieza", Symbol = "pz", Dimension = "Count", FactorToBase = 1m, Description = "Pieza" },
        new UnitOfMeasure { Id = PackageId, Name = "Paquete", Symbol = "pq", Dimension = "Count", FactorToBase = 1m, Description = "Paquete" },
        new UnitOfMeasure { Id = BoxId, Name = "Caja", Symbol = "cj", Dimension = "Count", FactorToBase = 1m, Description = "Caja" },
        new UnitOfMeasure { Id = DozenId, Name = "Docena", Symbol = "doc", Dimension = "Count", FactorToBase = 12m, Description = "Doce unidades" }
    };
}
