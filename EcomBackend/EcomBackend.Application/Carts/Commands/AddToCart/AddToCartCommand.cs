using MediatR;

namespace EcomBackend.Application.Carts.Commands.AddToCart;

public record AddToCartCommand(int UserId, int ProductId, int Quantity) : IRequest<CartDto>;
