using EcomBackend.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcomBackend.Application.Carts.Queries.GetMyCart;

public class GetMyCartQueryHandler : IRequestHandler<GetMyCartQuery, CartDto>
{
    private readonly IApplicationDbContext _context;

    public GetMyCartQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CartDto> Handle(GetMyCartQuery request, CancellationToken cancellationToken)
    {
        var dto = await _context.Carts
            .Where(c => c.UserId == request.UserId)
            .Select(c => new CartDto
            {
                Id = c.Id,
                UserId = c.UserId,
                Items = c.Items.Select(i => new CartItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return dto ?? new CartDto { UserId = request.UserId };
    }
}
