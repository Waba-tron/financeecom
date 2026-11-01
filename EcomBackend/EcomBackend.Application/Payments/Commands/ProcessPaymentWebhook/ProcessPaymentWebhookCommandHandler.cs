using EcomBackend.Application.Common.Interfaces;
using EcomBackend.Domain.Entities;
using EcomBackend.Domain.Payments;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcomBackend.Application.Payments.Commands.ProcessPaymentWebhook;

public class ProcessPaymentWebhookCommandHandler
    : IRequestHandler<ProcessPaymentWebhookCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentGateway _paymentGateway;

    public ProcessPaymentWebhookCommandHandler(
        IApplicationDbContext context,
        IPaymentGateway paymentGateway)
    {
        _context = context;
        _paymentGateway = paymentGateway;
    }

    public async Task<Unit> Handle(ProcessPaymentWebhookCommand request, CancellationToken cancellationToken)
    {
        var evt = _paymentGateway.ParseWebhookEvent(request.Payload, request.SignatureHeader);

        if (evt.Type == PaymentWebhookEventType.Unknown || string.IsNullOrEmpty(evt.SessionId))
            return Unit.Value;

        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.SessionId == evt.SessionId, cancellationToken);

        if (payment is null)
            return Unit.Value;

        // Idempotency: don't re-process terminal states.
        if (payment.Status is PaymentStatus.Succeeded or PaymentStatus.Failed)
            return Unit.Value;

        switch (evt.Type)
        {
            case PaymentWebhookEventType.CheckoutCompleted:
                payment.Status = PaymentStatus.Succeeded;
                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.Id == payment.OrderId, cancellationToken);
                if (order is not null)
                {
                    order.Status = OrderStatus.Paid;

                    // Clear the user's cart only now that payment has succeeded.
                    var cart = await _context.Carts
                        .Include(c => c.Items)
                        .FirstOrDefaultAsync(c => c.UserId == order.UserId, cancellationToken);

                    if (cart is not null && cart.Items.Count > 0)
                    {
                        _context.CartItems.RemoveRange(cart.Items);
                        cart.UpdatedDate = DateTime.UtcNow;
                    }
                }
                break;

            case PaymentWebhookEventType.CheckoutExpired:
            case PaymentWebhookEventType.PaymentFailed:
                payment.Status = PaymentStatus.Failed;
                var failedOrder = await _context.Orders
                    .FirstOrDefaultAsync(o => o.Id == payment.OrderId, cancellationToken);
                if (failedOrder is not null)
                    failedOrder.Status = OrderStatus.Cancelled;
                break;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
