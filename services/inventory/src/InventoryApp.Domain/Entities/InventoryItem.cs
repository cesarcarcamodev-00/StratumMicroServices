namespace InventoryApp.Domain.Entities;

public class InventoryItem : BaseEntity
{
    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    public string Location { get; set; } = "Almacén Principal";
    public decimal QuantityOnHand { get; set; }
}
