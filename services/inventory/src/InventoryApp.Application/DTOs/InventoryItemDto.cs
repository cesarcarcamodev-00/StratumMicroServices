namespace InventoryApp.Application.DTOs;

public class InventoryItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal QuantityOnHand { get; set; }
}
