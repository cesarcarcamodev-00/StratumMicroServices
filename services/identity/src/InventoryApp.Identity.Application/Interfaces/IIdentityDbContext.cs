using InventoryApp.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Identity.Application.Interfaces;

public interface IIdentityDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<UserPermission> UserPermissions { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
