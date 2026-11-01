using MediatR;

namespace EcomBackend.Application.Carts.Commands.UpdateCartItem;

public record UpdateCartItemCommand(int UserId, int ProductId, int Quantity) : IRequest<CartDto?>;
