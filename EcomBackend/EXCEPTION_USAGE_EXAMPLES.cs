// EXAMPLE USAGE OF GLOBAL EXCEPTION HANDLING
// How to use the new exception classes in your handlers

// ============================================
// Example 1: Resource Not Found
// ============================================

using EcomBackend.Application.Common.Exceptions;

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, OrderDto>
{
    private readonly IApplicationDbContext _context;

    public GetOrderQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OrderDto> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders.FindAsync(request.OrderId, cancellationToken);

        if (order is null)
            throw new ResourceNotFoundException(nameof(Order), request.OrderId);

        return MapToDto(order);
    }
}
// Response: 404 with "Order with identifier '123' was not found."


// ============================================
// Example 2: Unauthorized Access
// ============================================

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public async Task<AuthDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        return CreateToken(user);
    }
}
// Response: 401 with "Invalid email or password."


// ============================================
// Example 3: Forbidden Access
// ============================================

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IApplicationDbContext _context;

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        // Check if current user is admin
        if (request.CurrentUserRole != "Admin")
            throw new ForbiddenException("Only administrators can delete users.");

        var user = await _context.Users.FindAsync(request.UserId, cancellationToken);
        if (user is null)
            throw new ResourceNotFoundException(nameof(User), request.UserId);

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
// Response: 403 with "Only administrators can delete users."


// ============================================
// Example 4: Custom Application Exception
// ============================================

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IApplicationDbContext _context;

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken);

        if (cart?.Items.Count == 0)
            throw new ApplicationException("Cannot create order: cart is empty.", "EMPTY_CART", 400);

        // Create order...
        return orderDto;
    }
}
// Response: 400 with "Cannot create order: cart is empty."


// ============================================
// NO TRY-CATCH NEEDED ANYMORE!
// ============================================
// The middleware catches ALL of these and:
// 1. Logs the full exception
// 2. Returns proper HTTP status code
// 3. Returns consistent JSON format
// 4. Hides internal details from client

// BEFORE (OLD WAY):
/*
[HttpPost("checkout")]
public async Task<IActionResult> Checkout()
{
    try
    {
        var cart = await GetCart();
        if (cart?.Items.Count == 0)
            return BadRequest(new { message = "Cart is empty" });

        var order = await CreateOrder(cart);
        return Ok(new { ... });
    }
    catch (ValidationException ex)
    {
        return BadRequest(new { message = "Validation failed.", errors = ex.Errors });
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(new { message = ex.Message });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Checkout failed");
        return StatusCode(500, new { message = "Internal server error" });
    }
}
*/

// AFTER (NEW WAY):
/*
[HttpPost("checkout")]
public async Task<IActionResult> Checkout()
{
    var cart = await GetCart();
    if (cart?.Items.Count == 0)
        throw new ApplicationException("Cannot checkout: cart is empty", "EMPTY_CART");

    var order = await CreateOrder(cart);
    return Ok(order);
}
*/
// Much cleaner! 🎉
