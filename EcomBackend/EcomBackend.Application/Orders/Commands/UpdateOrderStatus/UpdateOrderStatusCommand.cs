using EcomBackend.Domain.Entities;
using MediatR;

namespace EcomBackend.Application.Orders.Commands.UpdateOrderStatus;

public record UpdateOrderStatusCommand(int OrderId, OrderStatus NewStatus) : IRequest<OrderDto>;
