using EcomBackend.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcomBackend.Application.ProductTypes.Queries.GetAllProductTypes;

public class GetAllProductTypesQueryHandler : IRequestHandler<GetAllProductTypesQuery, List<ProductTypeDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllProductTypesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductTypeDto>> Handle(GetAllProductTypesQuery request, CancellationToken cancellationToken)
    {
        return await _context.ProductTypes
            .Where(t => t.IsActive)
            .OrderBy(t => t.Name)
            .Select(t => new ProductTypeDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description
            })
            .ToListAsync(cancellationToken);
    }
}
