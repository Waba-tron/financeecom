using EcomBackend.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcomBackend.Application.Carts.Commands.ClearCart;

public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand>
{
    private readonly IApplicationDbContext _context;

    public ClearCartCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken);

        if (cart == null) return;
        if (cart.Items.Count == 0) return;

        _context.CartItems.RemoveRange(cart.Items);
        cart.UpdatedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
