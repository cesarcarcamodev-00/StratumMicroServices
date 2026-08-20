namespace InventoryApp.Application.DTOs;

public class UnitOfMeasureDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string Dimension { get; set; } = "Count";
    public decimal FactorToBase { get; set; } = 1m;
    public bool IsBaseUnit { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int ProductCount { get; set; }
}
