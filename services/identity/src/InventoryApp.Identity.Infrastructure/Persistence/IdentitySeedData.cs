using InventoryApp.Identity.Domain.Entities;
using InventoryApp.Identity.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Identity.Infrastructure.Persistence;

public static class IdentitySeedData
{
    public static async Task InitializeAsync(IdentityDbContext context)
    {
        await EnsurePermissionsAsync(context);
        await EnsureRolesAsync(context);
        await EnsureAdminUserAsync(context);
    }

    private static async Task EnsurePermissionsAsync(IdentityDbContext context)
    {
        var existingPermissions = await context.Permissions.ToListAsync();
        var allowedNames = PermissionConstants.All.Select(p => p.Name).ToHashSet();

        foreach (var permission in existingPermissions.Where(p => !allowedNames.Contains(p.Name)).ToList())
            context.Permissions.Remove(permission);

        foreach (var (name, module, description) in PermissionConstants.All)
        {
            var existing = existingPermissions.FirstOrDefault(p => p.Name == name);
            if (existing == null)
            {
                context.Permissions.Add(new Permission { Name = name, Module = module, Description = description });
            }
            else if (existing.Description != description || existing.Module != module)
            {
                existing.Description = description;
                existing.Module = module;
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task EnsureRolesAsync(IdentityDbContext context)
    {
        var permissions = await context.Permissions.ToListAsync();
        var adminPermissionNames = PermissionConstants.All.Select(p => p.Name).ToHashSet();
        var managerPermissionNames = new[]
        {
            PermissionConstants.InventoryView,
            PermissionConstants.InventoryProductsManage,
            PermissionConstants.InventoryStocksManage,
            PermissionConstants.InventoryMovementsView,
            PermissionConstants.InventoryReportsView,
            PermissionConstants.InventoryUnitsManage,
            PermissionConstants.InventorySettingsManage,
            PermissionConstants.InventoryRulesManage
        }.ToHashSet();
        var viewerPermissionNames = new[]
        {
            PermissionConstants.InventoryView,
            PermissionConstants.InventoryMovementsView,
            PermissionConstants.InventoryReportsView
        }.ToHashSet();

        await EnsureRoleAsync(context, "Admin", "Acceso total al sistema", adminPermissionNames, permissions);
        await EnsureRoleAsync(context, "Manager", "Gestión del módulo de inventario", managerPermissionNames, permissions);
        await EnsureRoleAsync(context, "Viewer", "Acceso de solo lectura", viewerPermissionNames, permissions);
    }

    private static async Task EnsureRoleAsync(IdentityDbContext context, string name, string description, ISet<string> permissionNames, List<Permission> permissions)
    {
        var role = await context.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Name == name);

        if (role == null)
        {
            role = new Role { Name = name, Description = description, IsActive = true };
            context.Roles.Add(role);
            await context.SaveChangesAsync();
        }
        else
        {
            role.Description = description;
            role.IsActive = true;
        }

        var wanted = permissions.Where(p => permissionNames.Contains(p.Name)).ToList();
        var wantedIds = wanted.Select(p => p.Id).ToHashSet();

        foreach (var rp in role.RolePermissions.Where(rp => !wantedIds.Contains(rp.PermissionId)).ToList())
            context.RolePermissions.Remove(rp);

        var existingIds = role.RolePermissions.Select(rp => rp.PermissionId).ToHashSet();

        foreach (var permission in wanted)
        {
            if (!existingIds.Contains(permission.Id))
                context.RolePermissions.Add(new RolePermission { Role = role, Permission = permission });
        }

        await context.SaveChangesAsync();
    }

    private static async Task EnsureAdminUserAsync(IdentityDbContext context)
    {
        var admin = await context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Username == "admin");

        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");

        if (admin == null)
        {
            var user = new User
            {
                Username = "admin",
                Email = "admin@inventario.com",
                PasswordHash = new PasswordHasher().Hash("Admin123!"),
                IsActive = true
            };

            if (adminRole != null)
                user.UserRoles.Add(new UserRole { User = user, Role = adminRole });

            context.Users.Add(user);
        }
        else
        {
            admin.IsActive = true;
            admin.UpdatedAt = DateTime.UtcNow;

            if (adminRole != null && !admin.UserRoles.Any(ur => ur.RoleId == adminRole.Id))
                context.UserRoles.Add(new UserRole { User = admin, Role = adminRole });
        }

        await context.SaveChangesAsync();
    }
}
