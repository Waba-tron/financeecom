using MediatR;

namespace EcomBackend.Application.Carts.Queries.GetMyCart;

public record GetMyCartQuery(int UserId) : IRequest<CartDto>;
