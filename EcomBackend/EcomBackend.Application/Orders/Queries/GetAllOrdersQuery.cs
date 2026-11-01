using MediatR;

namespace EcomBackend.Application.Orders.Queries;

public record GetAllOrdersQuery : IRequest<List<OrderDto>>;
