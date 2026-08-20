using FluentValidation;
using InventoryApp.Application.Common.Models;
using InventoryApp.Application.Interfaces;
using InventoryApp.Domain.Entities;
using InventoryApp.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Application.Features.Inventory.Commands;

public record AdjustStockCommand : IRequest<Result>
{
    public Guid ProductId { get; init; }
    public decimal QuantityChange { get; init; }
    public MovementType MovementType { get; init; } = MovementType.Adjustment;
    public string? Notes { get; init; }
    public string? ReferenceType { get; init; }
    public Guid? ReferenceId { get; init; }
    public string? Location { get; init; }
    public string? CreatedBy { get; init; }
}

public class AdjustStockCommandValidator : AbstractValidator<AdjustStockCommand>
{
    public AdjustStockCommandValidator()
    {
        RuleFor(v => v.ProductId).NotEmpty().WithMessage("'Producto' es requerido");
        RuleFor(v => v.QuantityChange).NotEqual(0).WithMessage("'Cambio de cantidad' no puede ser cero");
    }
}

public class AdjustStockCommandHandler : IRequestHandler<AdjustStockCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public AdjustStockCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(AdjustStockCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

        if (product == null)
            return Result.Failure("Producto no encontrado");

        var location = request.Location ?? "Almacén Principal";
        var inventoryItem = await _context.InventoryItems
            .FirstOrDefaultAsync(i => i.ProductId == request.ProductId && i.Location == location, cancellationToken);

        if (inventoryItem == null)
        {
            inventoryItem = new InventoryItem
            {
                ProductId = request.ProductId,
                Location = location,
                QuantityOnHand = 0
            };
            _context.InventoryItems.Add(inventoryItem);
        }

        var before = inventoryItem.QuantityOnHand;
        inventoryItem.QuantityOnHand = Math.Round(inventoryItem.QuantityOnHand + request.QuantityChange, 2, MidpointRounding.AwayFromZero);

        if (inventoryItem.QuantityOnHand < 0)
            return Result.Failure("Stock insuficiente");

        var movement = new InventoryMovement
        {
            ProductId = request.ProductId,
            MovementType = request.MovementType,
            Quantity = Math.Abs(request.QuantityChange),
            QuantityBefore = before,
            QuantityAfter = inventoryItem.QuantityOnHand,
            ReferenceType = request.ReferenceType,
            ReferenceId = request.ReferenceId,
            Notes = request.Notes,
            CreatedBy = request.CreatedBy
        };

        _context.InventoryMovements.Add(movement);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("Stock ajustado exitosamente");
    }
}
