using InventoryApp.Application.Common.Models;
using InventoryApp.Application.DTOs;
using InventoryApp.Application.Interfaces;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Application.Features.InventoryMovements.Queries;

public record GetMovementsByProductQuery : IRequest<PagedResult<InventoryMovementDto>>
{
    public Guid? ProductId { get; init; }
    public string? MovementType { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}

public class GetMovementsByProductQueryHandler : IRequestHandler<GetMovementsByProductQuery, PagedResult<InventoryMovementDto>>
{
    private readonly IApplicationDbContext _context;

    public GetMovementsByProductQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<InventoryMovementDto>> Handle(GetMovementsByProductQuery request, CancellationToken cancellationToken)
    {
        var query = _context.InventoryMovements
            .Include(m => m.Product)
            .AsQueryable();

        if (request.ProductId.HasValue)
            query = query.Where(m => m.ProductId == request.ProductId.Value);

        if (!string.IsNullOrWhiteSpace(request.MovementType))
            query = query.Where(m => m.MovementType.ToString() == request.MovementType);

        if (request.FromDate.HasValue)
            query = query.Where(m => m.CreatedAt >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(m => m.CreatedAt <= request.ToDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectToType<InventoryMovementDto>()
            .ToListAsync(cancellationToken);

        return new PagedResult<InventoryMovementDto>(items, totalCount, request.Page, request.PageSize);
    }
}
