using MediatR;

namespace EcomBackend.Application.Auth.Commands.Register;

public record RegisterCommand(string Email, string Password, string DisplayName)
    : IRequest<AuthResponse>;
