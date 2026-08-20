using FluentValidation;
using InventoryApp.Identity.Application.Common.Models;
using InventoryApp.Identity.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Identity.Application.Features.Roles.Commands;

public record CreateRoleCommand : IRequest<Result<Guid>>
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public List<Guid> PermissionIds { get; init; } = new();
}

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(v => v.Name).NotEmpty().WithMessage("'Nombre' es requerido").MaximumLength(100).WithMessage("'Nombre' no puede exceder 100 caracteres");
    }
}

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<Guid>>
{
    private readonly IIdentityDbContext _context;

    public CreateRoleCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        if (await _context.Roles.AnyAsync(r => r.Name == request.Name, cancellationToken))
            return Result<Guid>.Failure("Ya existe un rol con ese nombre");

        var role = new InventoryApp.Identity.Domain.Entities.Role
        {
            Name = request.Name,
            Description = request.Description,
            IsActive = true
        };

        foreach (var permissionId in request.PermissionIds.Distinct())
            role.RolePermissions.Add(new InventoryApp.Identity.Domain.Entities.RolePermission { Role = role, PermissionId = permissionId });

        _context.Roles.Add(role);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(role.Id, "Rol creado exitosamente");
    }
}
