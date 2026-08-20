using InventoryApp.Application.DTOs;
using InventoryApp.Application.Interfaces;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Application.Features.UnitsOfMeasure.Queries;

public record GetAllUnitsQuery : IRequest<List<UnitOfMeasureDto>>
{
    public string? Dimension { get; init; }
    public bool IncludeInactive { get; init; }
}

public class GetAllUnitsQueryHandler : IRequestHandler<GetAllUnitsQuery, List<UnitOfMeasureDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllUnitsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<UnitOfMeasureDto>> Handle(GetAllUnitsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.UnitsOfMeasure
            .Include(u => u.Products)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Dimension))
            query = query.Where(u => u.Dimension == request.Dimension);

        if (!request.IncludeInactive)
            query = query.Where(u => u.IsActive);

        var items = await query
            .OrderBy(u => u.Dimension)
            .ThenBy(u => u.IsBaseUnit ? 0 : 1)
            .ThenBy(u => u.Name)
            .ProjectToType<UnitOfMeasureDto>()
            .ToListAsync(cancellationToken);

        return items;
    }
}
