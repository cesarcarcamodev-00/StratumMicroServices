using InventoryApp.Identity.Application.Common.Models;
using InventoryApp.Identity.Application.Interfaces;
using MediatR;

namespace InventoryApp.Identity.Application.Features.Roles.Commands;

public record DeleteRoleCommand(Guid Id) : IRequest<Result>;

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Result>
{
    private readonly IIdentityDbContext _context;

    public DeleteRoleCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _context.Roles.FindAsync(new object?[] { request.Id }, cancellationToken);
        if (role == null)
            return Result.Failure("Rol no encontrado");

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("Rol eliminado");
    }
}
