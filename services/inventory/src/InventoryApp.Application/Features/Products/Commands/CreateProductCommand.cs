using FluentValidation;
using InventoryApp.Application.Common.Models;
using InventoryApp.Application.Interfaces;
using InventoryApp.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Application.Features.Products.Commands;

public record CreateProductCommand : IRequest<Result<Guid>>
{
    public string Name { get; init; } = string.Empty;
    public string SKU { get; init; } = string.Empty;
    public string? Description { get; init; }
    public Guid UnitId { get; init; }
    public decimal ReorderLevel { get; init; } = 10;
}

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(v => v.Name).NotEmpty().WithMessage("'Nombre' es requerido").MaximumLength(200).WithMessage("'Nombre' no puede exceder 200 caracteres");
        RuleFor(v => v.SKU).NotEmpty().WithMessage("'SKU' es requerido").MaximumLength(50).WithMessage("'SKU' no puede exceder 50 caracteres");
        RuleFor(v => v.UnitId).NotEmpty().WithMessage("'Unidad de medida' es requerida");
    }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var entity = request.Adapt<Product>();

        entity.InventoryItems.Add(new InventoryItem
        {
            ProductId = entity.Id,
            Location = "Almacén Principal",
            QuantityOnHand = 0
        });

        _context.Products.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(entity.Id, "Producto creado exitosamente");
    }
}
