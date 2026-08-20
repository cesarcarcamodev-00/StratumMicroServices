using FluentValidation;
using InventoryApp.Identity.Application.Common.Models;
using InventoryApp.Identity.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Identity.Application.Features.Users.Commands;

public record CreateUserCommand : IRequest<Result<Guid>>
{
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public bool IsActive { get; init; } = true;
    public List<Guid> RoleIds { get; init; } = new();
    public List<Guid> PermissionIds { get; init; } = new();
}

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(v => v.Username).NotEmpty().WithMessage("'Usuario' es requerido")
            .MinimumLength(3).WithMessage("'Usuario' debe tener al menos 3 caracteres")
            .MaximumLength(100).WithMessage("'Usuario' no puede exceder 100 caracteres");
        RuleFor(v => v.Email).NotEmpty().WithMessage("'Email' es requerido").EmailAddress().WithMessage("'Email' no es un correo electrónico válido");
        RuleFor(v => v.Password).NotEmpty().WithMessage("'Contraseña' es requerida")
            .MinimumLength(6).WithMessage("'Contraseña' debe tener al menos 6 caracteres");
    }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    private readonly IIdentityDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(IIdentityDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await _context.Users.AnyAsync(u => u.Username == request.Username, cancellationToken))
            return Result<Guid>.Failure("El nombre de usuario ya existe");
        if (await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
            return Result<Guid>.Failure("El correo electrónico ya existe");

        var user = new InventoryApp.Identity.Domain.Entities.User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            IsActive = request.IsActive
        };

        foreach (var roleId in request.RoleIds.Distinct())
            user.UserRoles.Add(new InventoryApp.Identity.Domain.Entities.UserRole { User = user, RoleId = roleId });

        foreach (var permissionId in request.PermissionIds.Distinct())
            user.UserPermissions.Add(new InventoryApp.Identity.Domain.Entities.UserPermission { User = user, PermissionId = permissionId });

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(user.Id, "Usuario creado exitosamente");
    }
}
