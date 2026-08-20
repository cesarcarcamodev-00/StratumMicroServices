using InventoryApp.Identity.Application.DTOs;
using InventoryApp.Identity.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Identity.Application.Features.Roles.Queries;

public record GetRolesQuery : IRequest<List<RoleDto>>
{
    public bool IncludeInactive { get; init; } = false;
}

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, List<RoleDto>>
{
    private readonly IIdentityDbContext _context;

    public GetRolesQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<List<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Roles
            .Include(r => r.UserRoles)
            .Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .AsQueryable();

        if (!request.IncludeInactive)
            query = query.Where(r => r.IsActive);

        var roles = await query.OrderBy(r => r.Name).ToListAsync(cancellationToken);

        return roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            IsActive = r.IsActive,
            UserCount = r.UserRoles.Count,
            PermissionIds = r.RolePermissions.Select(rp => rp.PermissionId).ToList(),
            Permissions = r.RolePermissions.Select(rp => new PermissionDto
            {
                Id = rp.Permission.Id,
                Name = rp.Permission.Name,
                Module = rp.Permission.Module,
                Description = rp.Permission.Description
            }).ToList()
        }).ToList();
    }
}
