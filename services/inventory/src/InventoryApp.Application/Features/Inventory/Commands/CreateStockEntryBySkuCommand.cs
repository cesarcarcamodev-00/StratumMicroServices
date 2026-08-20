using FluentValidation;
using InventoryApp.Application.Common.Models;
using InventoryApp.Application.Interfaces;
using InventoryApp.Domain.Entities;
using InventoryApp.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Application.Features.Inventory.Commands;

public record StockEntryResult
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string SKU { get; init; } = string.Empty;
    public string? UnitSymbol { get; init; }
    public decimal NewQuantity { get; init; }
    public Guid MovementId { get; init; }
}

public record CreateStockEntryBySkuCommand : IRequest<Result<StockEntryResult>>
{
    public string Sku { get; init; } = string.Empty;
    public decimal Weight { get; init; }
    public string? Notes { get; init; }
    public Guid? ReferenceId { get; init; }
    public string? CreatedBy { get; init; }
}

public class CreateStockEntryBySkuCommandValidator : AbstractValidator<CreateStockEntryBySkuCommand>
{
    public CreateStockEntryBySkuCommandValidator()
    {
        RuleFor(v => v.Sku).NotEmpty().WithMessage("'SKU' es requerido").MaximumLength(50).WithMessage("'SKU' no puede exceder 50 caracteres");
        RuleFor(v => v.Weight).GreaterThan(0).WithMessage("'Peso' debe ser mayor a 0");
    }
}

public class CreateStockEntryBySkuCommandHandler : IRequestHandler<CreateStockEntryBySkuCommand, Result<StockEntryResult>>
{
    private readonly IApplicationDbContext _context;

    public CreateStockEntryBySkuCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<StockEntryResult>> Handle(CreateStockEntryBySkuCommand request, CancellationToken cancellationToken)
    {
        var sku = request.Sku.Trim();
        var product = await _context.Products
            .Include(p => p.Unit)
            .FirstOrDefaultAsync(p => p.SKU.ToLower() == sku.ToLower(), cancellationToken);

        if (product == null)
            return Result<StockEntryResult>.Failure("SKU no encontrado", "NOT_FOUND");

        if (!product.IsActive)
            return Result<StockEntryResult>.Failure("El producto está inactivo", "INACTIVE");

        var weight = Math.Round(request.Weight, 2, MidpointRounding.AwayFromZero);
        var location = "Almacén Principal";
        var inventoryItem = await _context.InventoryItems
            .FirstOrDefaultAsync(i => i.ProductId == product.Id && i.Location == location, cancellationToken);

        if (inventoryItem == null)
        {
            inventoryItem = new InventoryItem
            {
                ProductId = product.Id,
                Location = location,
                QuantityOnHand = 0
            };
            _context.InventoryItems.Add(inventoryItem);
        }

        var before = inventoryItem.QuantityOnHand;
        inventoryItem.QuantityOnHand = Math.Round(inventoryItem.QuantityOnHand + weight, 2, MidpointRounding.AwayFromZero);

        var movement = new InventoryMovement
        {
            ProductId = product.Id,
            MovementType = MovementType.In,
            Quantity = weight,
            QuantityBefore = before,
            QuantityAfter = inventoryItem.QuantityOnHand,
            ReferenceType = "PESA",
            ReferenceId = request.ReferenceId,
            Notes = request.Notes,
            CreatedBy = request.CreatedBy
        };

        _context.InventoryMovements.Add(movement);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<StockEntryResult>.Success(new StockEntryResult
        {
            ProductId = product.Id,
            ProductName = product.Name,
            SKU = product.SKU,
            UnitSymbol = product.Unit?.Symbol,
            NewQuantity = inventoryItem.QuantityOnHand,
            MovementId = movement.Id
        }, "Stock ingresado exitosamente");
    }
}
