using EcomBackend.Application.Common.Interfaces;
using EcomBackend.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcomBackend.Application.Carts.Commands.AddToCart;

public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, CartDto>
{
    private readonly IApplicationDbContext _context;

    public AddToCartCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CartDto> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            ?? throw new InvalidOperationException($"Product {request.ProductId} not found.");

        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken);

        if (cart == null)
        {
            cart = new Cart { UserId = request.UserId };
            _context.Carts.Add(cart);
        }

        var existing = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
        if (existing != null)
        {
            existing.Quantity += request.Quantity;
            existing.UnitPrice = product.Price;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                ProductId = product.Id,
                Quantity = request.Quantity,
                UnitPrice = product.Price
            });
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
