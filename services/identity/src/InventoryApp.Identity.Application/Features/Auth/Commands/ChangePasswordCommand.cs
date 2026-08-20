using FluentValidation;
using InventoryApp.Identity.Application.Common.Models;
using InventoryApp.Identity.Application.Interfaces;
using MediatR;

namespace InventoryApp.Identity.Application.Features.Auth.Commands;

public record ChangePasswordCommand : IRequest<Result>
{
    public Guid UserId { get; init; }
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(v => v.CurrentPassword).NotEmpty().WithMessage("'Contraseña actual' es requerida");
        RuleFor(v => v.NewPassword).NotEmpty().WithMessage("'Nueva contraseña' es requerida")
            .MinimumLength(6).WithMessage("'Nueva contraseña' debe tener al menos 6 caracteres");
    }
}

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IIdentityDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordCommandHandler(IIdentityDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(new object?[] { request.UserId }, cancellationToken);
        if (user == null)
            return Result.Failure("Usuario no encontrado");

        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            return Result.Failure("La contraseña actual es incorrecta");

        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("Contraseña cambiada exitosamente");
    }
}
