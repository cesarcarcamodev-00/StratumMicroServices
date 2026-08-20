using InventoryApp.Application.Common.Models;
using InventoryApp.Application.DTOs;
using InventoryApp.Application.Interfaces;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Application.Features.Inventory.Queries;

public record GetProductStockQuery(Guid ProductId) : IRequest<Result<List<InventoryItemDto>>>;

public class GetProductStockQueryHandler : IRequestHandler<GetProductStockQuery, Result<List<InventoryItemDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetProductStockQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<InventoryItemDto>>> Handle(GetProductStockQuery request, CancellationToken cancellationToken)
    {
        var items = await _context.InventoryItems
            .Include(i => i.Product)
            .Where(i => i.ProductId == request.ProductId)
            .ProjectToType<InventoryItemDto>()
            .ToListAsync(cancellationToken);

        return Result<List<InventoryItemDto>>.Success(items);
    }
}
