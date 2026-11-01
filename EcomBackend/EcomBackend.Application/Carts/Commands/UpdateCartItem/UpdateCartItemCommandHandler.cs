using EcomBackend.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcomBackend.Application.Carts.Commands.UpdateCartItem;

public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, CartDto?>
{
    private readonly IApplicationDbContext _context;

    public UpdateCartItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CartDto?> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken);

        if (cart == null)
            return null;

        var item = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);

        if (item == null)
            return null;

        if (request.Quantity == 0)
        {
            _context.CartItems.Remove(item);
        }
        else
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
                ?? throw new InvalidOperationException($"Product {request.ProductId} not found.");

            item.Quantity = request.Quantity;
            item.UnitPrice = product.Price;
        }

        cart.UpdatedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return await _context.Carts
            .Where(c => c.Id == cart.Id)
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
            .FirstAsync(cancellationToken);
    }
}
