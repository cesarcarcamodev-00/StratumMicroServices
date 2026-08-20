namespace InventoryApp.Domain.Entities;

public class UnitOfMeasure : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string Dimension { get; set; } = "Count";
    public decimal FactorToBase { get; set; } = 1m;
    public bool IsBaseUnit { get; set; }
    public string? Description { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
