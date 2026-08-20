namespace InventoryApp.Identity.Application.DTOs;

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int UserCount { get; set; }
    public List<Guid> PermissionIds { get; set; } = new();
    public List<PermissionDto> Permissions { get; set; } = new();
}
