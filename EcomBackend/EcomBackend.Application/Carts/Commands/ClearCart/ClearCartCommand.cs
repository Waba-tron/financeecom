using MediatR;

namespace EcomBackend.Application.Carts.Commands.ClearCart;

public record ClearCartCommand(int UserId) : IRequest;
