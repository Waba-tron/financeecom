using EcomBackend.Application.Common.Interfaces;
using EcomBackend.Application.Payments;
using EcomBackend.Domain.Entities;
using EcomBackend.Domain.Payments;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcomBackend.Application.Orders.Commands.Checkout;

public class CheckoutCommandHandler : IRequestHandler<CheckoutCommand, OrderDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IPaymentRepository _paymentRepository;

    public CheckoutCommandHandler(
        IApplicationDbContext context,
        IPaymentGateway paymentGateway,
        IPaymentRepository paymentRepository)
    {
        _context = context;
        _paymentGateway = paymentGateway;
        _paymentRepository = paymentRepository;
    }

    public async Task<OrderDto> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken)
            ?? throw new InvalidOperationException("Cart is empty.");

        if (cart.Items.Count == 0)
            throw new InvalidOperationException("Cart is empty.");

        var order = new Order
        {
            UserId = request.UserId,
            Status = OrderStatus.Pending,
            CreatedDate = DateTime.UtcNow,
            Items = cart.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        order.Total = order.Items.Sum(i => i.UnitPrice * i.Quantity);

        // The cart is intentionally left intact here. It is only cleared once
        // payment succeeds (handled in ProcessPaymentWebhookCommandHandler), so
        // a user who cancels on the payment page keeps their cart contents.
        _context.Orders.Add(order);

        await _context.SaveChangesAsync(cancellationToken);

        // 🔥 Create Stripe Checkout Session
        var (sessionId, checkoutUrl) = await _paymentGateway.CreateCheckoutSessionAsync(
            order.Id,
            (long)(order.Total * 100),
            "usd",
            cancellationToken
        );

        // 🔥 Save Payment with SessionId
        var payment = new Payment(
            order.Id,
            (long)(order.Total * 100),
            "usd",
            sessionId
        );

        await _paymentRepository.AddAsync(payment, cancellationToken);

        // 🔥 RETURN CHECKOUT URL TO CLIENT
        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            Status = order.Status,
            Total = order.Total,
            CreatedDate = order.CreatedDate,
            CheckoutUrl = checkoutUrl,
            Items = order.Items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };
    }
}
