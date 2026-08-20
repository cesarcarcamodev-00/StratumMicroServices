using InventoryApp.Application.Common.Models;
using InventoryApp.Application.DTOs;
using InventoryApp.Application.Interfaces;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Application.Features.Products.Queries;

public record GetAllProductsQuery : IRequest<PagedResult<ProductDto>>
{
    public string? Search { get; init; }
    public bool? IsActive { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string SortBy { get; init; } = "Name";
    public bool SortAscending { get; init; } = true;
}

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, PagedResult<ProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Products
            .Include(p => p.Unit)
            .Include(p => p.InventoryItems)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(search) ||
                                     p.SKU.ToLower().Contains(search));
        }

        if (request.IsActive.HasValue)
            query = query.Where(p => p.IsActive == request.IsActive.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        query = request.SortBy.ToLower() switch
        {
            "name" => request.SortAscending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
            "sku" => request.SortAscending ? query.OrderBy(p => p.SKU) : query.OrderByDescending(p => p.SKU),
            "createdat" => request.SortAscending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
            _ => query.OrderBy(p => p.Name)
        };

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectToType<ProductDto>()
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductDto>(items, totalCount, request.Page, request.PageSize);
    }
}
