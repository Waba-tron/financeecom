using EcomBackend.Application.Common.Exceptions;
using EcomBackend.Application.Common.Interfaces;
using EcomBackend.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcomBackend.Application.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;

    public RegisterCommandHandler(IApplicationDbContext context, IPasswordHasher hasher, IJwtTokenService jwt)
    {
        _context = context;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var exists = await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);
        if (exists)
            throw new ConflictException("Email already registered.", "EMAIL_ALREADY_REGISTERED");

        var user = new User
        {
            Email = email,
            PasswordHash = _hasher.Hash(request.Password),
            DisplayName = request.DisplayName,
            Role = "Customer"
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            Token = _jwt.CreateToken(user.Id, user.Email, user.Role),
            DisplayName = user.DisplayName
        };
    }
}
