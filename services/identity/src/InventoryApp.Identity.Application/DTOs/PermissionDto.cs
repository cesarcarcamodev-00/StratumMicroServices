namespace InventoryApp.Identity.Application.DTOs;

public class PermissionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Module { get; set; } = "General";
    public string? Description { get; set; }
}
