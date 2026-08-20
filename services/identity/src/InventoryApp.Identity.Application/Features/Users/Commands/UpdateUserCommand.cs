using FluentValidation;
using InventoryApp.Identity.Application.Common.Models;
using InventoryApp.Identity.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Identity.Application.Features.Users.Commands;

public record UpdateUserCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string? Username { get; init; }
    public string? Email { get; init; }
    public bool? IsActive { get; init; }
    public List<Guid> RoleIds { get; init; } = new();
    public List<Guid> PermissionIds { get; init; } = new();
}

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty().WithMessage("'Id' es requerido");
        RuleFor(v => v.Username).MaximumLength(100).WithMessage("'Usuario' no puede exceder 100 caracteres");
        RuleFor(v => v.Email).EmailAddress().When(v => !string.IsNullOrEmpty(v.Email)).WithMessage("'Email' no es un correo electrónico válido");
    }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public UpdateUserCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .Include(u => u.UserPermissions)
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (user == null)
            return Result.Failure("Usuario no encontrado");

        if (!string.IsNullOrWhiteSpace(request.Username) && request.Username != user.Username)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username && u.Id != request.Id, cancellationToken))
                return Result.Failure("El nombre de usuario ya existe");
            user.Username = request.Username;
        }

        if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email && u.Id != request.Id, cancellationToken))
                return Result.Failure("El correo electrónico ya existe");
            user.Email = request.Email;
        }

        if (request.IsActive.HasValue)
            user.IsActive = request.IsActive.Value;

        user.UserRoles.Clear();
        foreach (var roleId in request.RoleIds.Distinct())
            user.UserRoles.Add(new InventoryApp.Identity.Domain.Entities.UserRole { User = user, RoleId = roleId });

        user.UserPermissions.Clear();
        foreach (var permissionId in request.PermissionIds.Distinct())
            user.UserPermissions.Add(new InventoryApp.Identity.Domain.Entities.UserPermission { User = user, PermissionId = permissionId });

        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("Usuario actualizado exitosamente");
    }
}
