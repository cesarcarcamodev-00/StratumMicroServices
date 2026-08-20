using InventoryApp.Application.Common.Models;
using InventoryApp.Application.DTOs;
using InventoryApp.Application.Interfaces;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Application.Features.Inventory.Queries;

public record GetLowStockProductsQuery : IRequest<List<ProductDto>>;

public class GetLowStockProductsQueryHandler : IRequestHandler<GetLowStockProductsQuery, List<ProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLowStockProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductDto>> Handle(GetLowStockProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _context.Products
            .Include(p => p.Unit)
            .Include(p => p.InventoryItems)
            .Where(p => p.IsActive && p.InventoryItems.Sum(i => i.QuantityOnHand) <= p.ReorderLevel)
            .ProjectToType<ProductDto>()
            .ToListAsync(cancellationToken);

        return products;
    }
}
