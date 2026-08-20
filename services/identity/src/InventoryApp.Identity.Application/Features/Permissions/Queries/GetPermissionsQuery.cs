using InventoryApp.Identity.Application.DTOs;
using InventoryApp.Identity.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Identity.Application.Features.Permissions.Queries;

public record GetPermissionsQuery : IRequest<List<PermissionDto>>
{
    public string? Module { get; init; }
}

public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, List<PermissionDto>>
{
    private readonly IIdentityDbContext _context;

    public GetPermissionsQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<List<PermissionDto>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Permissions.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Module))
            query = query.Where(p => p.Module == request.Module);

        var permissions = await query.OrderBy(p => p.Module).ThenBy(p => p.Name).ToListAsync(cancellationToken);

        return permissions.Select(p => new PermissionDto
        {
            Id = p.Id,
            Name = p.Name,
            Module = p.Module,
            Description = p.Description
        }).ToList();
    }
}
