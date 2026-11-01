using MediatR;

namespace EcomBackend.Application.Orders.Queries;

public record GetMyOrdersQuery(int UserId) : IRequest<List<OrderDto>>;

