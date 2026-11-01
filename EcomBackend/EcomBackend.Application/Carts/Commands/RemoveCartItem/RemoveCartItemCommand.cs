using MediatR;

namespace EcomBackend.Application.Carts.Commands.RemoveCartItem;

public record RemoveCartItemCommand(int UserId, int ProductId) : IRequest<CartDto?>;
