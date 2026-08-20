using InventoryApp.Identity.Application.Common.Models;
using InventoryApp.Identity.Application.DTOs;
using InventoryApp.Identity.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Identity.Application.Features.Users.Queries;

public record GetUsersQuery : IRequest<PagedResult<UserDto>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Search { get; init; }
}

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserDto>>
{
    private readonly IIdentityDbContext _context;

    public GetUsersQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.UserPermissions).ThenInclude(up => up.Permission)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(u => u.Username.Contains(search) || u.Email.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(u => u.Username)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var dtos = items.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            Roles = u.UserRoles.Where(ur => ur.Role.IsActive).Select(ur => ur.Role.Name).ToList(),
            RoleIds = u.UserRoles.Select(ur => ur.RoleId).ToList(),
            Permissions = u.UserPermissions.Select(up => up.Permission.Name).ToList(),
            PermissionIds = u.UserPermissions.Select(up => up.PermissionId).ToList()
        }).ToList();

        return new PagedResult<UserDto>(dtos, totalCount, request.Page, request.PageSize);
    }
}
