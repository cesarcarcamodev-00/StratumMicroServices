using FluentValidation;
using InventoryApp.Application.Common.Models;
using InventoryApp.Application.Interfaces;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Application.Features.Products.Commands;

public record UpdateProductCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public Guid UnitId { get; init; }
    public decimal ReorderLevel { get; init; }
    public bool? IsActive { get; init; }
}

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty().WithMessage("'Id' es requerido");
        RuleFor(v => v.Name).NotEmpty().WithMessage("'Nombre' es requerido").MaximumLength(200).WithMessage("'Nombre' no puede exceder 200 caracteres");
        RuleFor(v => v.UnitId).NotEmpty().WithMessage("'Unidad de medida' es requerida");
    }
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (entity == null)
            return Result.Failure("Producto no encontrado");

        var isActive = request.IsActive ?? entity.IsActive;
        request.Adapt(entity);
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = isActive;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success("Producto actualizado exitosamente");
    }
}
