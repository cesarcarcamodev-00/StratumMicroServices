using FluentValidation;
using InventoryApp.Identity.Application.Common.Models;
using InventoryApp.Identity.Application.DTOs;
using InventoryApp.Identity.Application.Interfaces;
using InventoryApp.Identity.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Identity.Application.Features.Auth.Commands;

public record LoginCommand : IRequest<Result<AuthResponse>>
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(v => v.Username).NotEmpty().WithMessage("'Usuario' no puede estar vacío");
        RuleFor(v => v.Password).NotEmpty().WithMessage("'Contraseña' no puede estar vacía");
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IIdentityDbContext _context;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IPasswordHasher _passwordHasher;

    public LoginCommandHandler(IIdentityDbContext context, IJwtTokenGenerator tokenGenerator, IPasswordHasher passwordHasher)
    {
        _context = context;
        _tokenGenerator = tokenGenerator;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ThenInclude(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .Include(u => u.UserPermissions).ThenInclude(up => up.Permission)
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive, cancellationToken);

        if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result<AuthResponse>.Failure("Usuario o contraseña inválidos");

        var (roles, permissions) = GetUserAccess(user);
        var (token, expiresAt) = _tokenGenerator.GenerateToken(user, roles, permissions);
        var refreshToken = _tokenGenerator.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<AuthResponse>.Success(new AuthResponse
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Roles = roles.ToArray(),
            Permissions = permissions.ToArray()
        });
    }

    internal static (List<string> roles, List<string> permissions) GetUserAccess(User user)
    {
        var roles = user.UserRoles
            .Where(ur => ur.Role.IsActive)
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToList();

        var permissions = new HashSet<string>(
            user.UserRoles.SelectMany(ur => ur.Role.RolePermissions.Select(rp => rp.Permission.Name)),
            StringComparer.OrdinalIgnoreCase);

        foreach (var up in user.UserPermissions.Where(up => up.Permission != null))
            permissions.Add(up.Permission.Name);

        return (roles, permissions.OrderBy(p => p).ToList());
    }
}
