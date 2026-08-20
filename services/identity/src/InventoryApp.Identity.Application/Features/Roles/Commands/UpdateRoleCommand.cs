using FluentValidation;
using InventoryApp.Identity.Application.Common.Models;
using InventoryApp.Identity.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Identity.Application.Features.Roles.Commands;

public record UpdateRoleCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public bool? IsActive { get; init; }
    public List<Guid> PermissionIds { get; init; } = new();
}

public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty().WithMessage("'Id' es requerido");
        RuleFor(v => v.Name).MaximumLength(100).When(v => !string.IsNullOrEmpty(v.Name)).WithMessage("'Nombre' no puede exceder 100 caracteres");
    }
}

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public UpdateRoleCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (role == null)
            return Result.Failure("Rol no encontrado");

        if (!string.IsNullOrWhiteSpace(request.Name) && request.Name != role.Name)
        {
            if (await _context.Roles.AnyAsync(r => r.Name == request.Name && r.Id != request.Id, cancellationToken))
                return Result.Failure("Ya existe un rol con ese nombre");
            role.Name = request.Name;
        }

        if (request.Description != null)
            role.Description = request.Description;

        if (request.IsActive.HasValue)
            role.IsActive = request.IsActive.Value;

        role.RolePermissions.Clear();
        foreach (var permissionId in request.PermissionIds.Distinct())
            role.RolePermissions.Add(new InventoryApp.Identity.Domain.Entities.RolePermission { Role = role, PermissionId = permissionId });

        role.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("Rol actualizado exitosamente");
    }
}
