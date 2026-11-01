using MediatR;

namespace EcomBackend.Application.Orders.Commands.Checkout;

public record CheckoutCommand(int UserId) : IRequest<OrderDto>;
